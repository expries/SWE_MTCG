using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using MTCG.Exceptions;

namespace MTCG.Server
{
    /// <summary>
    /// parse HTTP requests
    /// </summary>
    public class RequestContext
    {
        /// <summary>
        /// HTTP verb (GET, POST, PUT, etc.)
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// requested/addressed resource
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// HTTP
        /// </summary>
        public string Protocol { get; set; }
        
        /// <summary>
        /// which HTTP version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// http basic auth token
        /// </summary>
        public string Authorization => GetAuthorization();

        /// <summary>
        /// attributes for request
        /// </summary>
        public Dictionary<string, string> Headers { get; }
        
        /// <summary>
        /// carries values of parameters in request path
        /// </summary>
        public Dictionary<string, string> PathParam { get; set; }
        
        /// <summary>
        /// request body
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// length of request body
        /// </summary>
        public int ContentLength => GetContentLength();
        
        /// <summary>
        /// for buffering characters to complete lines
        /// </summary>
        private string LineBuffer { get; set; }

        /// <summary>
        /// content length as specified in header
        /// </summary>
        private int ContentLengthHeader => GetContentLengthFromHeader();

        /// <summary>
        /// true if end of header was encountered
        /// </summary>
        private bool HeaderComplete { get; set; }
        
        /// <summary>
        /// true if first line of http request was read
        /// </summary>
        private bool RequestLineWasRead { get; set; }

        /// <summary>
        /// initialise request
        /// </summary>
        public RequestContext()
        {
            PathParam = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Content = null;
            Method = null;
            Protocol = null;
            Path = null;
            Version = null;
            HeaderComplete = false;
            RequestLineWasRead = false;
            LineBuffer = "";
        }

        /// <summary>
        /// print request details
        /// </summary>
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

        /// <summary>
        /// read and parse request
        /// </summary>
        /// <param name="request"></param>
        public void Read(string request)
        {
            foreach (char c in request)
            {
                if (HeaderComplete)
                {
                    ReadContent(c);
                }
                else
                {
                    ReadHeader(c);
                }
            }
        }

        /// <summary>
        /// read character of the body of a http request
        /// </summary>
        /// <param name="c"></param>
        /// <exception cref="BadRequestException"></exception>
        private void ReadContent(char c)
        {
            Content += c;
            
            // supplied content length is incorrect => error
            if (ContentLength > ContentLengthHeader)
            {
                throw new BadRequestException("Supplied content length is incorrect.");
            }
        }

        /// <summary>
        /// read character for header segment of http request
        /// </summary>
        /// <param name="c"></param>
        private void ReadHeader(char c)
        {
            if (!LineCompleted(c))
            {
                return;
            }
            
            if (!RequestLineWasRead)
            {
                ReadRequestLine(LineBuffer);
                LineBuffer = "";
                return;
            }
                        
            if (LineBuffer.Length == 0)
            {
                HeaderComplete = true;
                return;
            }
                        
            ReadHeaderAttribute(LineBuffer);
            LineBuffer = "";
        }

        /// <summary>
        /// parse header attribute line and add it to header
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="BadRequestException"></exception>
        private void ReadHeaderAttribute(string line)
        {
            string[] attribute = line.Split(": ");
            
            if (attribute.Length < 2)
            {
                throw new BadRequestException(
                    "Invalid header attribute: " + string.Join(": ", attribute));
            }
            
            string name = attribute[0];
            string value = attribute[1];
            Headers[name] = value;
        }
        
        /// <summary>
        /// parse first line of request to obtain protocol info
        /// assuming request line has format "[method] [path] [protocol]/[version]"
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="BadRequestException"></exception>
        private void ReadRequestLine(string line)
        {
            string[] requestInfo = line.Split(" ");
            
            if (requestInfo.Length < 3)
            {
                throw new BadRequestException("Could not parse request line: " + line);
            }
            
            Method = requestInfo[0];
            Path = requestInfo[1];

            if (Path.EndsWith("/"))  // remove trailing '/' from path
            {
                Path = Path.Substring(0, Path.Length - 1);
            }
            
            if (!Path.StartsWith("/"))  // force leading '/' in path
            {
                Path = "/" + Path;
            }
            
            string[] protocolInfo = requestInfo[2].Split("/");

            if (protocolInfo.Length < 2) {
                throw new BadRequestException("Could not parse protocol/version: " + requestInfo[2]);
            }
            
            Protocol = protocolInfo[0];
            Version = protocolInfo[1];
            RequestLineWasRead = true;
        }
        
        /// <summary>
        /// add char to buffer, return true if the line is completed
        /// swallows newline and carriage-return characters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool LineCompleted(char c)
        {
            switch (c)
            {
                case '\r':
                    return false;
                case '\n':
                    return true;
                default:
                    LineBuffer += c;
                    return false;
            }
        }
        
        /// <summary>
        /// return content length
        /// </summary>
        /// <returns></returns>
        private int GetContentLength()
        {
            return Content == null ? 0 : System.Text.Encoding.UTF8.GetByteCount(Content);
        }
        
        /// <summary>
        /// return content-length declared in header
        /// </summary>
        /// <returns></returns>
        private int GetContentLengthFromHeader()
        {
            if (!Headers.ContainsKey("Content-Length"))
            {
                return 0;
            }
            
            string contentLength = Headers["Content-Length"];
            return int.Parse(contentLength);
        }

        /// <summary>
        /// return authorization token from header attribute
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        private string GetAuthorization()
        {
            if (!Headers.ContainsKey("Authorization"))
            {
                return null;
            }

            string[] parts = Headers["Authorization"].Split(" ");

            if (parts.Length < 2)
            {
                throw new BadRequestException("Invalid Authorization header attribute.");
            }
            
            return parts[1];
        }
    }
}