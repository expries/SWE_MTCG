using System;
using MTCG.Server;

namespace MTCG.Exceptions
{
    public class MethodNotImplementedException : HttpException
    {
        public MethodNotImplementedException()
            : base (HttpStatus.NotImplemented) {}

        public MethodNotImplementedException(string message)
            : base(HttpStatus.NotImplemented, message) {}

        public MethodNotImplementedException(string message, Exception inner)
            : base(HttpStatus.NotImplemented, message, inner) {}
    }
}