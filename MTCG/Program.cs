using System;
using System.Collections.Generic;
using MTCG.Controller;
using MTCG.Database;
using MTCG.Database.Entity;
using MTCG.Mapper;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Resource.Cards;
using MTCG.Server;
using MTCG.Service;
using Npgsql;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager.MapEnum<Element>("element_type");
            DatabaseManager.MapEnum<CardType>("card_type");
            DatabaseManager.MapEnum<MonsterType>("monster_type");
            
            var db = new DatabaseManager("localhost", "postgres", "postgres", "postgres");

            // create repositories
            var userRepository = new UserRepository(db);
            var cardRepository = new CardRepository();
            var packageRepository = new PackageRepository();
            
            // create services
            var cardService = new CardService();
            var packageService = new PackageService();
            var userService = new UserService();
            
            // create controllers that utilise created repositories
            var cardController = new CardController(cardRepository);
            var userController = new UserController(UserService);
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
                return userController.Register(WebMapper.MapJsonTo<RegistrationRequest>(context.Content));
            });
            
            server.RegisterRoute("GET", "/users/{username}", context =>
            {
                return userController.Get(context.PathParam["username"]);
            });

            // SESSIONS
            server.RegisterRoute("POST", "/sessions", context =>
            {
                return userController.Login(WebMapper.MapJsonTo<LoginRequest>(context.Content));
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
                return cardController.Get(WebMapper.MapToGuid(cardId));
            });
            
            // PACKAGES
            server.RegisterRoute("POST", "/packages", context =>
            {
                var cardRequests = WebMapper.MapJsonTo<List<CardCreationRequest>>(context.Content);
                return packageController.Create(cardRequests);
            });
            
            server.RegisterRoute("GET", "/packages", context =>
            {
                return packageController.GetAll();
            });
            
            server.RegisterRoute("GET", "/packages/{packageId}", context =>
            {
                string packageId = context.PathParam["packageId"];
                return packageController.Get(WebMapper.MapToGuid(packageId));
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
