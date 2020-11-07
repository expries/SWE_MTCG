using System;
using System.Collections.Generic;

namespace MTCG.Server
{
    // build and output HTTP responses
    public class ResponseContext
    {
        public string Protocol { get; set; }  // protocol used
        public string Version { get; set; }  // protocol version
        public HttpStatus Status { get; set; }  // HTTP Status
        public Dictionary<string, string> Headers { get; }  // header attributes
        public string Content { get; set; }  // payload
        public int ContentLength => GetContentLength();
        public string ContentType  // e.g. application/json
        {
            get => GetContentType();
            set => SetContentType(value);
        }

        public ResponseContext()
        {
            Protocol = "HTTP";
            Version = "1.1";
            Status = HttpStatus.Ok;
            Headers = new Dictionary<string, string>();
            Content = null;
        }

        // create plaintext response
        public new string ToString()
        {
            if (Protocol == null || Version == null)
            {
                throw new Exception("Protocol and version are mandatory.");
            }
            
            Headers["Content-Length"] = ContentLength.ToString();
            
            // set charset for plaintext responses to utf-8
            if (ContentType == MediaType.Plaintext || ContentType == MediaType.Html)
            {
                Headers["Content-Type"] += "; charset=utf-8";
            }
            
            string requestLine = GenerateRequestLine();
            string header = HeaderToString();
            string content = ContentToString();
            
            return $"{requestLine}\r\n{header}\r\n{content}";
        }
 
        // get first line of request ...
        private string GenerateRequestLine()
        {
            string statusCode = Status.GetCode().ToString();
            string statusPhrase = Status.GetPhrase();
            return $"{Protocol}/{Version} {statusCode} {statusPhrase}";
        }

        // get header key-value pairs as string to be used in a request
        private string HeaderToString()
        {
            string result = "";
            foreach ((string key, string value) in Headers)
            {
                result += $"{key}: {value}\r\n";
            }
            return result;
        }

        // return content or empty string if content is not set
        private string ContentToString()
        {
            return Content ?? "";
        }
        
        // return content length
        private int GetContentLength()
        {
            return Content == null ? 0 : System.Text.Encoding.UTF8.GetByteCount(Content);
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