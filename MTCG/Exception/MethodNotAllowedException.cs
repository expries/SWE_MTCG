using MTCG.Server;

namespace MTCG.Exception
{
    public class MethodNotAllowedException : HttpException
    {
        public MethodNotAllowedException()
            : base (HttpStatus.MethodNotAllowed) {}

        public MethodNotAllowedException(string message)
            : base(HttpStatus.MethodNotAllowed, message) {}

        public MethodNotAllowedException(string message, System.Exception inner)
            : base(HttpStatus.MethodNotAllowed, message, inner) {}
    }
}