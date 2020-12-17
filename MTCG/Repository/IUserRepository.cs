using System;
using System.Collections.Generic;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public interface IUserRepository
    {
        public List<User> GetAllUsers();

        public User GetUser(string username);

        public bool CheckCredentials(string username, string password);

        public List<Guid> GetPackageIds(string username);

        public bool AcquirePackage(string username, Guid packageId);

        public void AddCoins(string username, int coins);

        public void EmptyDeck(string username);
        
        public bool SetDeck(string username, List<Guid> cardIds);
        
        void AddPackageToUser(string username, Package packageToAcquire);
        
        List<Card> GetDeck(string username);
        
        User CreateUser(User user);
    }
}