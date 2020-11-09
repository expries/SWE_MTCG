using System;
using MTCG.Server;

namespace MTCG.Exceptions
{
    public class ConflictException : HttpException
    {
        public ConflictException() 
            : base (HttpStatus.Conflict) {}
        
        public ConflictException(string message)
            : base (HttpStatus.Conflict, message) {}

        public ConflictException(string message, Exception inner)
            : base (HttpStatus.Conflict, message, inner) {}
    }
}