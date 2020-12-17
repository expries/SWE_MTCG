using System;
using System.Collections.Generic;
using MTCG.Exception;

namespace MTCG.Server
{
    // parse HTTP requests
    public class RequestContext
    {
        // HTTP GET, POST, PUT, etc.
        public string Method { get; set; }
        
        // requested/addressed resource
        public string Path { get; set; }
        
        // HTTP
        public string Protocol { get; set; }
        
        // which HTTP version
        public string Version { get; set; } 
        
        public Dictionary<string, string> Headers { get; }
        
        public Dictionary<string, string> PathParam { get; set; }
        
        public string Content { get; set; }
        
        public int ContentLength => GetContentLength();
        
        public string ContentType
        {
            get => GetContentType();
            set => SetContentType(value);
        }
        
        // get content length that is specified in header
        private int ContentLengthHeader => GetContentLengthFromHeader();
        
        private string Buffer { get; set; }
        
        // true if end of header was encountered
        private bool HeaderComplete { get; set; }

        public RequestContext()
        {
            PathParam = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Content = null;
            Method = null;
            Protocol = null;
            Path = null;
            Version = null;
            Buffer = "";
            HeaderComplete = false;
        }

        // print request details
        public void PrintProperties()
        {
            Console.WriteLine("");
            Console.WriteLine("Request: ");
            Console.WriteLine("Method = " + Method);
            Console.WriteLine("Path = " + Path);
            Console.WriteLine("Protocol = " + Protocol);
            Console.WriteLine("Version = " + Version);
            Console.WriteLine("Content: " + Content);
            Console.WriteLine("Header:");
            
            foreach ((string key, string value) in Headers)
            {
                Console.WriteLine($"+ {key}: {value}");
            }
            
            Console.WriteLine("");
        }

        // read request from string
        public void Read(string request)
        {
            foreach (char c in request)
            {
                if (!HeaderComplete)
                {
                    AddToHeader(c);
                    continue;
                }

                Content += c;
                
                // supplied content length is incorrect => error
                if (ContentLength > ContentLengthHeader)
                {
                    throw new BadRequestException("Supplied content length is incorrect.");
                }
            }
        }

        // build header by adding one char at a time
        private void AddToHeader(char c)
        {
            // accumulate characters up to a newline (discard the newline)
            if (!BufferToLine(c))
            {
                return;
            }

            if (Buffer.Length == 0)  // end of header is implied by empty line
            {
                HeaderComplete = true;
            }
            else if (!RequestLineWasRead())
            {
                ReadRequestLine();
            }
            else  // any other header line contains an attribute
            {
                ReadHeaderAttribute();
            }
            
            Buffer = "";
        }

        // add char to buffer
        // return true if line is complete, false otherwise
        private bool BufferToLine(char c)
        {
            switch (c)
            {
                case '\n':
                    return true;
                case '\r':
                    return false;
                default:
                    Buffer += c;
                    return false;
            }
        }

        // if method or protocol is not set, request line was not read yet
        private bool RequestLineWasRead()
        {
            return Method != null && Protocol != null;
        }

        // parse header attribute line and add it to header
        private void ReadHeaderAttribute()
        {
            var attribute = Buffer.Split(": ");
            if (attribute.Length < 2)
            {
                throw new BadRequestException("Invalid header attribute: " + attribute);
            }
            string name = attribute[0];
            string value = attribute[1];
            Headers[name] = value;
        }
        
        // parse first line of request to obtain protocol info
        // assuming request line has format "<method> <path> <protocol>/<version>"
        private void ReadRequestLine()
        {
            var requestInfo = Buffer.Split(" ");
            
            if (requestInfo.Length < 3)
            {
                throw new BadRequestException("Could not parse request line: " + Buffer);
            }
            
            Method = requestInfo[0];
            Path = requestInfo[1];
            var protocolInfo = requestInfo[2].Split("/");
            
            if (Path.EndsWith("/"))  // remove trailing '/' from path
            {
                Path = Path.Substring(0, Path.Length - 1);
            }
            if (!Path.StartsWith("/"))  // force leading '/' in path
            {
                Path = "/" + Path;
            }

            if (protocolInfo.Length < 2) {
                throw new BadRequestException("Could not parse protocol/version: " + requestInfo[2]);
            }
            
            Protocol = protocolInfo[0];
            Version = protocolInfo[1];
        }
        
        // return content length
        private int GetContentLength()
        {
            return Content == null ? 0 : System.Text.Encoding.UTF8.GetByteCount(Content);
        }
        
        // get content-length declared in header
        private int GetContentLengthFromHeader()
        {
            if (!Headers.ContainsKey("Content-Length"))
            {
                return 0;
            }
            string contentLength = Headers["Content-Length"];
            return int.Parse(contentLength);
        }
        
        // set content type in header
        private void SetContentType(string contentType)
        {
            Headers["Content-Type"] = contentType;
        }

        // get content type from header
        private string GetContentType()
        {
            return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : null;
        }
    }
}