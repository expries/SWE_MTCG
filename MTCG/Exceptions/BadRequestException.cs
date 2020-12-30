using MTCG.Server;

namespace MTCG.Exceptions
{
    public class BadRequestException : HttpException
    {
        public BadRequestException() 
            : base (HttpStatus.BadRequest) {}

        public BadRequestException(string message)
            : base (HttpStatus.BadRequest, message) {}

        public BadRequestException(string message, System.Exception inner)
            : base (HttpStatus.BadRequest, message, inner) {}
    }
}