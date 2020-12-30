using MTCG.Server;

namespace MTCG.Exceptions
{
    public class HttpException : System.Exception
    {
        public HttpStatus Status { get; }

        public HttpException(HttpStatus status)
            : base("")
        {
            Status = status;
        }

        public HttpException(HttpStatus status, string message)
            : base(message)
        {
            Status = status;
        }

        public HttpException(HttpStatus status, string message, System.Exception inner)
            : base(message, inner)
        {
            Status = status;
        }
    }
}