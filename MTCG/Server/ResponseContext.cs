using System.Collections.Generic;
using System.Text;

namespace MTCG.Server
{
    /// <summary>
    /// build and output HTTP responses
    /// </summary>
    public class ResponseContext
    {
        /// <summary>
        /// protocol used
        /// </summary>
        public string Protocol { get; set; }
        
        /// <summary>
        /// protocol version
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// HTTP Status
        /// </summary>
        public HttpStatus Status { get; set; }
        
        /// <summary>
        /// header attributes
        /// </summary>
        public Dictionary<string, string> Headers { get; }
        
        /// <summary>
        /// payload
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// length of the request body
        /// </summary>
        public int ContentLength => GetContentLength();
        
        /// <summary>
        /// for the header attribute 'ContentType'
        /// </summary>
        public string ContentType
        {
            get => GetContentType();
            set => SetContentType(value);
        }

        /// <summary>
        /// initialise a response with HTTP Status 200 Ok and no content
        /// </summary>
        public ResponseContext()
        {
            Protocol = "HTTP";
            Version = "1.1";
            Status = HttpStatus.Ok;
            Headers = new Dictionary<string, string>();
            Content = null;
        }

        /// <summary>
        /// initialise a response with a given HTTP Status and content (optional, default null)
        /// </summary>
        /// <param name="status"></param>
        /// <param name="content"></param>
        public ResponseContext(HttpStatus status, string content = null) : this()
        {
            Status = status;
            Content = content;
        }

        /// <summary>
        /// create plaintext response
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public new string ToString()
        {
            if (Protocol == null || Version == null)
            {
                throw new System.Exception("Protocol and version are mandatory.");
            }
            
            Headers["Content-Length"] = ContentLength.ToString();
            
            // set charset for plaintext responses to utf-8
            if (ContentType == MediaType.Plaintext || ContentType == MediaType.Html)
            {
                Headers["Content-Type"] += "; charset=utf-8";
            }

            var builder = new StringBuilder();
            builder.Append(GenerateRequestLine());
            builder.Append("\r\n");
            builder.Append(HeaderToString());
            builder.Append("\r\n");
            builder.Append(ContentToString());

            return builder.ToString();
        }
 
        /// <summary>
        /// get first line of request
        /// </summary>
        /// <returns></returns>
        private string GenerateRequestLine()
        {
            string statusCode = Status.GetCode().ToString();
            string statusPhrase = Status.GetPhrase();
            return $"{Protocol}/{Version} {statusCode} {statusPhrase}";
        }

        /// <summary>
        /// get header key-value pairs as string to be used in a request
        /// </summary>
        /// <returns></returns>
        private string HeaderToString()
        {
            string result = "";
            foreach ((string key, string value) in Headers)
            {
                result += $"{key}: {value}\r\n";
            }
            return result;
        }

        /// <summary>
        /// return content or empty string if content is not set
        /// </summary>
        /// <returns></returns>
        private string ContentToString()
        {
            return Content ?? "";
        }
        
        /// <summary>
        /// return content length
        /// </summary>
        /// <returns></returns>
        private int GetContentLength()
        {
            return Content == null ? 0 : Encoding.UTF8.GetByteCount(Content);
        }

        /// <summary>
        /// set content type in header
        /// </summary>
        /// <param name="contentType"></param>
        private void SetContentType(string contentType)
        {
            Headers["Content-Type"] = contentType;
        }

        /// <summary>
        /// get content type from header
        /// </summary>
        /// <returns></returns>
        private string GetContentType()
        {
            return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : null;
        }
    }
}