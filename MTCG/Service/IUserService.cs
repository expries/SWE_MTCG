using System;
using System.Collections.Generic;
using MTCG.ActionResult;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public interface IUserService
    {
        public User GetUser(string username);

        public List<User> GetAllUsers();

        public bool VerifyLogin(string username, string password);
        
        public bool VerifyUser(string username);

        public List<Card> GetDeck(string username);
        
        public ActionResult<User> CreateUser(string username, string password);

        public ActionResult<Package> AcquirePackage(string username);

        public ActionResult<List<Card>> SetDeck(string username, List<Guid> cardIds);
    }
}