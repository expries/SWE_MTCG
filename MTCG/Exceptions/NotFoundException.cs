using System;
using MTCG.Server;

namespace MTCG.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException()
            : base (HttpStatus.NotFound) {}

        public NotFoundException(string message)
            : base(HttpStatus.NotFound, message) {}

        public NotFoundException(string message, Exception inner)
            : base(HttpStatus.NotFound, message, inner) {}
    }
}