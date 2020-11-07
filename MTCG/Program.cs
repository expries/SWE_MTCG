using MTCG.Exceptions;
using MTCG.Server;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebServer();

            server.RegisterRoute("GET", "/", context => new ResponseContext
            {
                Status = HttpStatus.Ok,
                Content = "Landing page!",
                ContentType = MediaType.Plaintext
            });

            server.Start();
            server.Listen("127.0.0.1", 48501);
        }
    }
}
