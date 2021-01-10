using System;
using System.Collections.Generic;
using System.Threading;
using MTCG.Controllers;
using MTCG.Database;
using MTCG.Domain.Cards;
using MTCG.Exceptions;
using MTCG.Mappers;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Server;
using Newtonsoft.Json;

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
            var cardRepository = new CardRepository(db);
            var packageRepository = new PackageRepository(db);
            var scoreRepository = new StatsRepository(db);
            var tradeRepository = new TradeRepository(db, userRepository, cardRepository);
            var gameRepository = new GameRepository();
            var messageRepository = new MessageRepository(db, userRepository);

            // create controllers that use repositories
            var userController = new UserController(userRepository, packageRepository, cardRepository, tradeRepository);
            var packageController = new PackageController(packageRepository, cardRepository, userRepository);
            var statsController = new StatsController(scoreRepository, userRepository);
            var tradeController = new TradeController(tradeRepository, userRepository, cardRepository);
            var gameController = new GameController(gameRepository, userRepository);
            var messageController = new MessageController(messageRepository, userRepository);

            // create server
            var server = new WebServer();

            // USERS
            server.RegisterRoute("GET", "/users", _ =>
            {
                return userController.GetAllUsers();
            });
            
            server.RegisterRoute("POST", "/users", context =>
            {
                var request = WebMapper.MapJsonTo<RegistrationRequest>(context.Content);
                return userController.RegisterUser(request);
            });
            
            server.RegisterRoute("GET", "/users/{username}", context =>
            {
                string token = context.Authorization;
                string username = context.PathParam["username"];
                return userController.GetUser(token, username);
            });
            
            server.RegisterRoute("PUT", "/users/{username}", context =>
            {
                string username = context.PathParam["username"];
                string token = context.Authorization;
                var userRequest = WebMapper.MapJsonTo<UserUpdateRequest>(context.Content);
                return userController.UpdateUser(token, username, userRequest);
            });

            // SESSIONS
            server.RegisterRoute("POST", "/sessions", context =>
            {
                var request = WebMapper.MapJsonTo<LoginRequest>(context.Content);
                return userController.Login(request);
            });
            
            // CARDS
            server.RegisterRoute("GET", "/cards", context =>
            {
                string token = context.Authorization;
                return userController.GetStack(token);
            });

            // PACKAGES
            server.RegisterRoute("POST", "/packages", context =>
            {
                var requests = WebMapper.MapJsonTo<List<CardCreationRequest>>(context.Content);
                string token = context.Authorization;
                return packageController.Create(token, requests);
            });
            
            server.RegisterRoute("GET", "/packages", _ =>
            {
                return packageController.GetAll();
            });
            
            server.RegisterRoute("GET", "/packages/{packageId}", context =>
            {
                string id = context.PathParam["packageId"];
                var packageId = WebMapper.MapToGuid(id);
                return packageController.Get(packageId);
            });
            
            // TRANSACTIONS
            server.RegisterRoute("POST", "/transactions/packages", context =>
            {
                string token = context.Authorization;
                return userController.AcquirePackage(token);
            });
            
            // DECK
            server.RegisterRoute("GET", "/deck", context =>
            {
                string token = context.Authorization;
                return userController.GetDeck(token);
            });
            
            server.RegisterRoute("GET", "/deck?format=plain", context =>
            {
                string token = context.Authorization;
                return userController.GetDeckPlaintext(token);
            });
            
            server.RegisterRoute("PUT", "/deck", context =>
            {
                string token = context.Authorization;
                var cardIds = WebMapper.MapJsonTo<List<Guid>>(context.Content);
                return userController.UpdateDeck(token, cardIds);
            });
            
            // STATS
            server.RegisterRoute("GET", "/stats", context =>
            {
                string token = context.Authorization;
                return statsController.GetStats(token);
            });
            
            // SCOREBOARD
            server.RegisterRoute("GET", "/score", context =>
            {
                string token = context.Authorization;
                return statsController.GetScoreboard(token);
            });
            
            // TRADES
            server.RegisterRoute("GET", "/tradings", context =>
            {
                string token = context.Authorization;
                return tradeController.GetTrades(token);
            });
            
            server.RegisterRoute("POST", "/tradings", context =>
            {
                string token = context.Authorization;
                var request = WebMapper.MapJsonTo<TradeCreationRequest>(context.Content);
                return tradeController.CreateTrade(token, request);
            });
            
            server.RegisterRoute("POST", "/tradings/{tradeId}", context =>
            {
                string token = context.Authorization;
                var tradeId = WebMapper.MapToGuid(context.PathParam["tradeId"]);
                var cardId = WebMapper.MapJsonTo<Guid>(context.Content);
                return tradeController.CommitTrade(token, tradeId, cardId);
            });
            
            server.RegisterRoute("DELETE", "/tradings/{tradeId}", context =>
            {
                string token = context.Authorization;
                var tradeId = WebMapper.MapToGuid(context.PathParam["tradeId"]);
                return tradeController.DeleteTrade(token, tradeId);
            });
            
            // BATTLES
            server.RegisterRoute("POST", "/battles", context =>
            {
                string token = context.Authorization;
                return gameController.PlayGame(token);
            });
            
            // CHATTING
            server.RegisterRoute("POST", "/messages", context =>
            {
                string token = context.Authorization;
                var request = WebMapper.MapJsonTo<MessageCreationRequest>(context.Content);
                return messageController.SendMessage(token, request);
            });
            
            server.RegisterRoute("GET", "/messages/{messageId}", context =>
            {
                string token = context.Authorization;
                var messageId = WebMapper.MapToGuid(context.PathParam["messageId"]);
                return messageController.GetMessage(token, messageId);
            });
            
            server.RegisterRoute("DELETE", "/messages/{messageId}", context =>
            {
                string token = context.Authorization;
                var messageId = WebMapper.MapToGuid(context.PathParam["messageId"]);
                return messageController.DeleteMessage(token, messageId);
            });
            
            server.RegisterRoute("GET", "/inbox", context =>
            {
                string token = context.Authorization;
                return messageController.GetInbox(token);
            });
            
            server.RegisterRoute("GET", "/chat/{username}", context =>
            {
                string token = context.Authorization;
                string chatPartner = context.PathParam["username"];
                return messageController.ReadConversation(token, chatPartner);
            });

            // EXTRA ENDPOINTS FOR DEVELOPMENT
            server.RegisterRoute("GET", "/wait", _ =>
            {
                Thread.Sleep(20000);
                return new ResponseContext(HttpStatus.Ok, "Waited!");
            });
            
            server.AddExceptionHandler(exception =>
            {
                Error error;
                HttpStatus status;
                
                if (exception is HttpException httpException)
                {
                    error = new Error(httpException.Message);
                    status = httpException.Status;
                }
                else
                {
                    Console.WriteLine(exception.Message);
                    error = new Error("An error has occurred.");
                    status = HttpStatus.InternalServerError;
                    throw exception;
                }
                
                string json = JsonConvert.SerializeObject(error);
                return new ResponseContext(status, json, MediaType.Json);
            });

            // start accepting clients
            server.Start();
            server.Listen("127.0.0.1", 10001);
        }
    }
}
