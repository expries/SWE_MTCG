using System.Collections.Generic;
using MTCG.Controller;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Server;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            // create repositories
            var userRepository = new UserRepository();
            var cardRepository = new CardRepository();
            var packageRepository = new PackageRepository();
            
            // create controllers that utilise created repositories
            var cardController = new CardController(cardRepository);
            var userController = new UserController(userRepository, cardRepository, packageRepository);
            var packageController = new PackageController(cardRepository, packageRepository);

            // create server
            var server = new WebServer();

            // USERS
            server.RegisterRoute("GET", "/users", context =>
            {
                return userController.GetAll();
            });
            
            server.RegisterRoute("POST", "/users", context =>
            {
                return userController.Register(MapJsonTo<RegistrationRequest>(context.Content));
            });
            
            server.RegisterRoute("GET", "/users/{username}", context =>
            {
                return userController.Get(context.PathParam["username"]);
            });

            // SESSIONS
            server.RegisterRoute("POST", "/sessions", context =>
            {
                return userController.Login(MapJsonTo<LoginRequest>(context.Content));
            });
            
            // CARDS
            server.RegisterRoute("GET", "/cards", context =>
            {
                return userController.GetCards(context.Headers["Authorization"]);
            });
            
            server.RegisterRoute("GET", "/cardsAll", context =>
            {
                return cardController.GetAll();
            });
            
            server.RegisterRoute("GET", "/cardsAll/{cardId}", context =>
            {
                string cardId = context.PathParam["cardId"];
                return cardController.Get(MapToGuid(cardId));
            });
            
            // PACKAGES
            server.RegisterRoute("POST", "/packages", context =>
            {
                var cardRequests = MapJsonTo<List<CardCreationRequest>>(context.Content);
                return packageController.Create(cardRequests);
            });
            
            server.RegisterRoute("GET", "/packages", context =>
            {
                return packageController.GetAll();
            });
            
            server.RegisterRoute("GET", "/packages/{packageId}", context =>
            {
                string packageId = context.PathParam["packageId"];
                return packageController.Get(MapToGuid(packageId));
            });
            
            // TRANSACTIONS
            server.RegisterRoute("POST", "/transactions/packages", context =>
            {
                return userController.AcquirePackage(context.Headers["Authorization"]);
            });

            // start accepting clients
            server.Start();
            server.Listen("127.0.0.1", 48501);
        }
    }
}
