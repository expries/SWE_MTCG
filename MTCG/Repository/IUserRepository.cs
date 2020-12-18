using System;
using System.Collections.Generic;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public interface IUserRepository
    {
        public User GetUser(string username);

        public List<User> GetAllUsers();

        public void AddCoins(string username, int coins);

        public void EmptyDeck(string username);

        public bool SetDeck(string username, List<Guid> cardIds);

        public void AddPackageToUser(string username, Package package);
        
        public List<Card> GetDeck(string username);

        public List<Card> GetStack(string username);

        public User CreateUser(User user);
    }
}