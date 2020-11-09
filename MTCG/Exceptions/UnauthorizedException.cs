using System;
using MTCG.Server;

namespace MTCG.Exceptions
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException()
            : base (HttpStatus.Unauthorized) {}

        public UnauthorizedException(string message)
            : base(HttpStatus.Unauthorized, message) {}

        public UnauthorizedException(string message, Exception inner)
            : base(HttpStatus.Unauthorized, message, inner) {}
    }
}