using MTCG.Server;

namespace MTCG.Exception
{
    public class NotFoundException : HttpException
    {
        public NotFoundException()
            : base (HttpStatus.NotFound) {}

        public NotFoundException(string message)
            : base(HttpStatus.NotFound, message) {}

        public NotFoundException(string message, System.Exception inner)
            : base(HttpStatus.NotFound, message, inner) {}
    }
}