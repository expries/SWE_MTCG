using MTCG.Server;

namespace MTCG.Exception
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException()
            : base (HttpStatus.Unauthorized) {}

        public UnauthorizedException(string message)
            : base(HttpStatus.Unauthorized, message) {}

        public UnauthorizedException(string message, System.Exception inner)
            : base(HttpStatus.Unauthorized, message, inner) {}
    }
}