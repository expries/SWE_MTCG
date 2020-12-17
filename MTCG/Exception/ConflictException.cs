using MTCG.Server;

namespace MTCG.Exception
{
    public class ConflictException : HttpException
    {
        public ConflictException() 
            : base (HttpStatus.Conflict) {}
        
        public ConflictException(string message)
            : base (HttpStatus.Conflict, message) {}

        public ConflictException(string message, System.Exception inner)
            : base (HttpStatus.Conflict, message, inner) {}
    }
}