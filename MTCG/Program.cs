using System;
using System.Collections.Generic;
using System.Threading;
using MTCG.Controller;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using MTCG.Mappers;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Server;

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
            var scoreRepository = new ScoreRepository(db);
            var tradeRepository = new TradeRepository(db, userRepository, cardRepository);
            var battleRepository = new BattleRepository();
            
            // create controllers that use repositories
            var cardController = new CardController(cardRepository);
            var userController = new UserController(userRepository, packageRepository, cardRepository);
            var packageController = new PackageController(packageRepository, cardRepository, userRepository);
            var statsController = new StatsController(scoreRepository, userRepository);
            var tradeController = new TradeController(tradeRepository, userRepository, cardRepository);

            var playerA = User.Create("playerA", "A").Value;
            var playerB = User.Create("playerB", "B").Value;

            var cardA1 = new FireSpell("FireSpell", 100);
            var cardA2 = new WaterSpell("WaterSpell", 120);
            var cardA3 = new Goblin("Goblin", 120);
            var cardA4 = new NormalSpell("NormalSpell", 120);
            var cardA5 = new Dragon("Dragon", 120);
            
            var cardB1 = new FireSpell("FireSpell", 100);
            var cardB2 = new WaterSpell("WaterSpell", 120);
            var cardB3 = new FireElf("FireElf", 120);
            var cardB4 = new Wizard("Wizard", 120);
            var cardB5 = new Knight("Knight", 120);

            playerA.AddToCollection(cardA1);
            playerA.AddToCollection(cardA2);
            playerA.AddToCollection(cardA3);
            playerA.AddToCollection(cardA4);
            playerA.AddToCollection(cardA5);

            playerA.AddToDeck(cardA1);
            playerA.AddToDeck(cardA2);
            playerA.AddToDeck(cardA3);
            playerA.AddToDeck(cardA4);
            playerA.AddToDeck(cardA5);
            
            playerB.AddToCollection(cardB1);
            playerB.AddToCollection(cardB2);
            playerB.AddToCollection(cardB3);
            playerB.AddToCollection(cardB4);
            playerB.AddToCollection(cardB5);
            
            playerB.AddToDeck(cardB1);
            playerB.AddToDeck(cardB2);
            playerB.AddToDeck(cardB3);
            playerB.AddToDeck(cardB4);
            playerB.AddToDeck(cardB5);

            var x = Battle.Create(playerA).Value;
            x.RegisterAsSecondPlayer(playerB);
            x.Play();
            Console.WriteLine(x.MetaInfo.Winner);
            Console.WriteLine(x.MetaInfo.ToString());
            return;

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
                string username = context.PathParam["username"];
                return userController.GetUser(username);
            });
            
            server.RegisterRoute("PUT", "/users/{username]", context =>
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
            
            server.RegisterRoute("POST", "/tradings/{trade}", context =>
            {
                string token = context.Authorization;
                var tradeId = WebMapper.MapToGuid(context.PathParam["trade"]);
                var cardId = WebMapper.MapToGuid(context.Content);
                return tradeController.CommitTrade(token, tradeId, cardId);
            });

            // EXTRA ENDPOINTS FOR DEVELOPMENT
            server.RegisterRoute("GET", "/cardsAll", _ =>
            {
                return cardController.GetAll();
            });
            
            server.RegisterRoute("GET", "/cardsAll/{cardId}", context =>
            {
                string cardId = context.PathParam["cardId"];
                return cardController.Get(WebMapper.MapToGuid(cardId));
            });
            
            server.RegisterRoute("GET", "/wait", context =>
            {
                Thread.Sleep(20000);
                return new ResponseContext(HttpStatus.Ok, "Waited!");
            });

            // start accepting clients
            server.Start();
            server.Listen("127.0.0.1", 48501);
        }
    }
}
