using System;
using MTCG.Server;

namespace MTCG.Exceptions
{
    public class MethodNotAllowedException : HttpException
    {
        public MethodNotAllowedException()
            : base (HttpStatus.MethodNotAllowed) {}

        public MethodNotAllowedException(string message)
            : base(HttpStatus.MethodNotAllowed, message) {}

        public MethodNotAllowedException(string message, Exception inner)
            : base(HttpStatus.MethodNotAllowed, message, inner) {}
    }
}