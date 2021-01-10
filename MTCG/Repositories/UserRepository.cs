using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public User GetById(Guid userId)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE userID = @UserId";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {UserId = userId});
            var user = MapEntity(entity);
            return user;
        }

        public User GetByUsername(string username)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE username = @Username";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {Username = username});
            var user = MapEntity(entity);
            return user;
        }

        public User GetByToken(string token)
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\" WHERE token = @Token";

            var entity = _db.QueryFirstOrDefault<UserEntity>(sql, new {Token = token});
            var user = MapEntity(entity);
            return user;
        }

        public List<User> GetAll()
        {
            const string sql = "SELECT userID, username, password, name, bio, image, " + 
                               "       token, coins, wins, draws, loses, elo " +
                               "FROM \"user\"";
            
            var entities = _db.Query<UserEntity>(sql);
            var users = entities.Select(MapEntity).ToList();
            return users;
        }

        public User Create(User user)
        {
            const string sql = "INSERT INTO \"user\" (userID, username, password, token, coins) " +
                               "VALUES (@UserId, @Username, @Password, @Token, @Coins)";
            
            _db.Execute(sql, new {
                UserId = user.Id, 
                Username = user.Username, 
                Password = user.Password, 
                Token = user.Token, 
                Coins = user.Coins
            });

            return GetById(user.Id);
        }
        
        public User Update(User user)
        {
            var storedUser = GetById(user.Id);
            UpdateData(user, storedUser);
            UpdateStack(user, storedUser);
            UpdateDeck(user, storedUser);
            return user;
        }
        
        private User MapEntity(UserEntity entity)
        {
            if (entity is null)
            {
                return null;
            }
            
            var user = User.Create(entity.Username, entity.Password, entity.Id, entity.Coins, 
                entity.Name, entity.Bio, entity.Image, entity.Wins, entity.Loses, entity.Draws, entity.Elo).Value;

            user.SetPasswordHash(entity.Password);
            var stack = GetStack(entity.Id);
            var deck = GetDeck(entity.Id);
            
            stack.ForEach(card => user.AddToStack(card));
            user.SetDeck(deck);
            return user;
        }
        
        private void UpdateData(User newUser, User storedUser)
        {
            // do not update if data is the same
            if (newUser.Username == storedUser.Username &&
                newUser.Password == storedUser.Password &&
                newUser.Token == storedUser.Token &&
                newUser.Coins == storedUser.Coins &&
                newUser.Elo == storedUser.Elo &&
                newUser.Wins == storedUser.Wins &&
                newUser.Loses == storedUser.Loses &&
                newUser.Draws == storedUser.Draws &&
                newUser.Name == storedUser.Name &&
                newUser.Bio == storedUser.Bio &&
                newUser.Image == storedUser.Image)
            {
                return;
            }
            
            SetUserData(newUser);
        }
        
        private void UpdateStack(User newUser, User storedUser)
        {
            var stackIds = newUser.Stack.Select(x => x.Id);
            var storedStackIds = storedUser.Stack.Select(x => x.Id);
            
            var removedCards = storedUser.Stack.Where(x => !stackIds.Contains(x.Id)).ToList();
            var newCards = newUser.Stack.Where(x => !storedStackIds.Contains(x.Id)).ToList();
            
            removedCards.ForEach(x => RemoveFromStack(newUser, x));
            newCards.ForEach(x => AddToStack(newUser, x));
        }

        private void UpdateDeck(User newUser, User storedUser)
        {
            var storedDeckIds = storedUser.Deck.Select(x => x.Id);

            // do not update if deck stayed the same
            if (newUser.Deck.All(x => storedDeckIds.Contains(x.Id)))
            {
                return;
            }
            
            SetDeck(newUser);
        }
        
        private void SetDeck(User user)
        {
            EmptyDeck(user);
            user.Deck.ForEach(card => AddToDeck(user, card));
        }
        
        private void SetUserData(User user)
        {
            const string sql = "UPDATE \"user\" " +
                               "SET username = @Username, " +
                               "    password = @Password, " +
                               "    token = @Token, " +
                               "    coins = @Coins, " +
                               "    elo = @Elo, " +
                               "    wins = @Wins, " +
                               "    loses = @Loses, " +
                               "    draws = @Draws, " + 
                               "    name = @Name, " + 
                               "    bio = @Bio, " + 
                               "    image = @Image " + 
                               "WHERE userID = @UserId";
            
            _db.Execute(sql, new {
                Username = user.Username, 
                Password = user.Password, 
                Token = user.Token, 
                Coins = user.Coins, 
                Elo = user.Elo, 
                Wins = user.Wins, 
                Loses = user.Loses, 
                Draws = user.Draws, 
                Name = user.Name, 
                Bio = user.Bio, 
                Image = user.Image,
                UserId = user.Id
            });
        }

        private void RemoveFromStack(User user, Card card)
        {
            const string sql = "DELETE FROM stack " +
                               "WHERE fk_userID = @UserId AND fk_cardID = @CardId";

            _db.Execute(sql, new {UserId = user.Id, CardId = card.Id});
        }

        private void AddToStack(User user, Card card)
        {
            const string sql = "INSERT INTO stack (fk_userID, fk_cardID) " +
                               "VALUES (@UserId, @CardId)";
            
            _db.Execute(sql, new {UserId = user.Id, CardId = card.Id});
        }

        private void EmptyDeck(User user)
        {
            const string sql = "DELETE FROM deck " + 
                               "WHERE fk_userID = @userID";
            
            _db.Execute(sql, new {userID = user.Id});
        }
        
        private void AddToDeck(User user, Card card)
        {
            const string sql = "INSERT INTO deck (fk_userID, fk_cardID) " +
                               "VALUES (@userID, @cardID)";
            
            _db.Execute(sql, new {userID = user.Id, cardID = card.Id});
        }
        
        private List<Card> GetDeck(Guid userId)
        {
            const string sql = "SELECT username, cardID, name, type, element, damage, fk_packageID, monsterType " +
                               "FROM user_deck " +
                               "WHERE userID = @UserId";
            
            var entities = _db.Query<CardEntity>(sql, new {UserId = userId});
            var cards = CardEntityMapper.MapIgnoreErrors(entities);
            return cards;
        }

        private List<Card> GetStack(Guid userId)
        {
            const string sql = "SELECT username, cardID, name, type, element, damage, fk_packageID, monsterType " +
                               "FROM user_stack " +
                               "WHERE userID = @UserId";
            
            var entities = _db.Query<CardEntity>(sql, new {UserId = userId});
            var cards = CardEntityMapper.MapIgnoreErrors(entities);
            return cards;
        }
    }
}