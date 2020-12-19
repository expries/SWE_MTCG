using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Database;
using MTCG.Database.Entity;
using MTCG.Mapper;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseManager _db;

        public UserRepository(DatabaseManager db)
        {
            _db = db;
        }

        public User GetUser(string username)
        {
            const string sql = "SELECT userID, username, password FROM \"user\" " +
                               "WHERE username = @username";

            var entity = _db.FetchFromQuery<UserEntity>(sql, new {Username = username}).FirstOrDefault();

            if (entity is null)
            {
                return null;
            }
            
            var userMapper = new UserEntityMapper();
            var user = userMapper.Map(entity);
            
            var stack = GetStack(username);
            var deck = GetDeck(username);
            stack.ForEach(card => user.AddCard(card));
            deck.ForEach(card => user.AddCardToDeck(card.Id));
            
            return user;
        }

        public List<User> GetAllUsers()
        {
            const string sql = "SELECT userID, username, password FROM \"user\"";
            var records = _db.FetchFromQuery<UserEntity>(sql);
            var userMapper = new UserEntityMapper();
            var userList = userMapper.Map(records).ToList();

            foreach (var user in userList)
            {
                var stack = GetStack(user.Username);
                var deck = GetDeck(user.Username);
                stack.ForEach(card => user.AddCard(card));
                deck.ForEach(card => user.AddCardToDeck(card.Id));
            }
            
            return userList;
        }

        public void AddCoins(string username, int coins)
        {
            const string sql = "UPDATE \"user\" SET coins = coins + @coins " +
                               "WHERE username = @username";
            
            _db.ExecuteQuery(sql, new {Username = username, Coins = coins});
        }

        public void EmptyDeck(string username)
        {
            const string sql = "DELETE FROM deck WHERE fk_userID = @userID";
            var user = GetUser(username);
            _db.ExecuteNonQuery(sql, new {userID = user.Id});
        }

        public bool SetDeck(string username, List<Guid> cardIds)
        {
            const string sql = "INSERT INTO deck (fk_userID, fk_cardID) " +
                               "VALUES (@userID, @cardID)";

            int affectedRows = 0;
            var user = GetUser(username);
            
            foreach (var id in cardIds)
            {
                affectedRows += _db.ExecuteNonQuery(sql, new {userID = user.Id, cardID = id});
            }

            return affectedRows == cardIds.Count;
        }

        public void AddPackageToUser(string username, Package package)
        {
            const string sql = "INSERT INTO collection (fk_userID, fk_packageID) " +
                               "VALUES (@userID, @packageID)";

            var user = GetUser(username);
            _db.ExecuteNonQuery(sql, new {userID = user.Id, package.Id});
        }

        public User CreateUser(User user)
        {
            const string sql = "INSERT INTO \"user\" (userID, username, password, coins) " +
                               "VALUES (@userID, @username, @password, @coins)";

            var userData = new
                {userID = user.Id, username = user.Username, password = user.Password, coins = user.Coins};
            _db.ExecuteNonQuery(sql, userData);

            return GetUser(user.Username);
        }
        
        private List<Card> GetDeck(string username)
        {
            const string sql = "SELECT username, cardid, name, type, element, damage, fk_packageid, monstertype " +
                               "FROM user_deck " +
                               "WHERE username = @username";

            var entities = _db.FetchFromQuery<CardEntity>(sql, new {Username = username});
            var cardMapper = new CardEntityMapper();
            var cards = cardMapper.Map(entities);
            return cards.ToList();
        }

        private List<Card> GetStack(string username)
        {
            const string sql = "SELECT username, cardid, name, type, element, damage, fk_packageid, monstertype " +
                               "FROM card_collection " +
                               "WHERE username = @username";
            
            var entities = _db.FetchFromQuery<CardEntity>(sql, new {Username = username});
            var cardMapper = new CardEntityMapper();
            var cards = cardMapper.Map(entities);
            return cards.ToList();
        }
    }
}