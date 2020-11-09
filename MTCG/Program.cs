using MTCG.Contracts;
using MTCG.Contracts.Requests;
using MTCG.Controllers;
using static MTCG.Mappers.Mapper;
using MTCG.Server;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WebServer();
            var userController = new UserController();

            server.RegisterRoute("POST", "/users", context =>
            {
                return userController.Register(MapJsonTo<RegistrationRequest>(context.Content));
            });
            server.RegisterRoute("POST", "/sessions", context =>
            {
                return userController.Login(MapJsonTo<LoginRequest>(context.Content));
            });

            server.Start();
            server.Listen("127.0.0.1", 48501);
        }
    }
}
