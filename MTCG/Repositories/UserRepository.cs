using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Entities;
using MTCG.Mappers;

namespace MTCG.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseManager _db;

        public UserRepository(DatabaseManager db)
        {
            _db = db;
        }

        public User GetUserById(Guid userId)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE userID = @userID";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {userID = userId});

            if (entity is null)
            {
                return null;
            }
            
            var user = UserEntityMapper.Map(entity);
            var stack = GetStack(entity.Username);
            var deck = GetDeck(entity.Username);
            
            stack.ForEach(card => user.AddToCollection(card));
            deck.ForEach(card => user.AddToDeck(card));

            return user;
        }

        public User GetUserByUsername(string username)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE username = @username";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {Username = username});

            if (entity is null)
            {
                return null;
            }
            
            var user = UserEntityMapper.Map(entity);
            var stack = GetStack(username);
            var deck = GetDeck(username);
            
            stack.ForEach(card => user.AddToCollection(card));
            deck.ForEach(card => user.AddToDeck(card));

            return user;
        }

        public User GetUserByToken(string token)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE token = @token";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {Token = token});
            return entity is null ? null : GetUserByUsername(entity.Username);
        }

        public List<User> GetAllUsers()
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\"";
            
            var entities = _db.Query<UserEntity>(sql);
            var users = UserEntityMapper.Map(entities);

            foreach (var user in users)
            {
                var stack = GetStack(user.Username);
                var deck = GetDeck(user.Username);
                stack.ForEach(card => user.AddToCollection(card));
                deck.ForEach(card => user.AddToDeck(card));
            }
            
            return users;
        }

        public User CreateUser(User user)
        {
            const string sql = "INSERT INTO \"user\" (userID, username, password, token, coins) " +
                               "VALUES (@userID, @username, @password, @token, @coins)";

            var userData = new {userID = user.Id, username = user.Username, password = user.Password, 
                token = user.Token, coins = user.Coins};
            
            _db.ExecuteNonQuery(sql, userData);

            return GetUserByUsername(user.Username);
        }
        
        public User UpdateUser(User user)
        {
            UpdateUserData(user);
            UpdateStack(user);
            SetDeck(user);
            return user;
        }

        private void UpdateUserData(User user)
        {
            const string sql = "UPDATE \"user\" " +
                               "SET name = @Name, " +
                               "    bio = @Bio, " +
                               "    image = @Image, " +
                               "    coins = @Coins, " +
                               "    wins = @Wins, " +
                               "    draws = @Draws, " +
                               "    loses = @Loses, " +
                               "    elo = @Elo " + 
                               "WHERE username = @Username";
            
            _db.ExecuteNonQuery(sql, new {
                Name = user.Name, 
                Bio = user.Bio, 
                Image = user.Image, 
                Coins = user.Coins, 
                Wins = user.Wins, 
                Draws = user.Draws, 
                Loses = user.Loses, 
                Elo = user.Elo, 
                Username = user.Username});
        }
        
        private void UpdateStack(User user)
        {
            var storedUser = GetUserByUsername(user.Username);
            var stackIds = user.Stack.Select(x => x.Id).ToList();
            var storedStackIds = storedUser.Stack.Select(x => x.Id).ToList();
            
            var removedCards = storedUser.Stack.Where(x => !stackIds.Contains(x.Id)).ToList();
            var newCards = user.Stack.Where(x => !storedStackIds.Contains(x.Id)).ToList();
            removedCards.ForEach(x => RemoveFromStack(user, x));
            newCards.ForEach(x => AddToStack(user, x));
        }

        private void AddToStack(User user, Card card)
        {
            const string sql = "INSERT INTO collection (fk_userID, fk_cardID) " +
                               "VALUES (@userID, @cardID)";
            
            _db.ExecuteNonQuery(sql, new {userID = user.Id, cardID = card.Id});
        }

        private void RemoveFromStack(User user, Card card)
        {
            const string sql = "DELETE FROM collection " +
                               "WHERE fk_userID = @userID AND fk_cardID = @cardID";

            _db.ExecuteNonQuery(sql, new {userID = user.Id, cardID = card.Id});
        }

        private void SetDeck(User user)
        {
            EmptyDeck(user);
            user.Deck.ForEach(card => AddToDeck(user, card));
        }

        private void AddToDeck(User user, Card card)
        {
            const string sql = "INSERT INTO deck (fk_userID, fk_cardID) " +
                               "VALUES (@userID, @cardID)";
            
            _db.ExecuteNonQuery(sql, new {userID = user.Id, cardID = card.Id});
        }

        private void EmptyDeck(User user)
        {
            const string sql = "DELETE FROM deck " + 
                               "WHERE fk_userID = @userID";
            
            _db.ExecuteNonQuery(sql, new {userID = user.Id});
        }

        private List<Card> GetDeck(string username)
        {
            const string sql = "SELECT username, cardID, name, type, element, damage, fk_packageID, monsterType " +
                               "FROM user_deck " +
                               "WHERE username = @username";
            
            var entities = _db.Query<CardEntity>(sql, new {Username = username});
            var cards = CardEntityMapper.Map(entities);
            return cards;
        }

        private List<Card> GetStack(string username)
        {
            const string sql = "SELECT username, cardID, name, type, element, damage, fk_packageID, monsterType " +
                               "FROM user_collection " +
                               "WHERE username = @username";
            
            var entities = _db.Query<CardEntity>(sql, new {Username = username});
            var cards = CardEntityMapper.Map(entities);
            return cards;
        }
    }
}