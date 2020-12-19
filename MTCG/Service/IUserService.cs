using System;
using System.Collections.Generic;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public interface IUserService
    {
        public OneOf<User, NotFound> GetUser(string username);

        public List<User> GetAllUsers();

        public bool CheckCredentials(string username, string password);
        
        public bool VerifyUser(string username);

        public OneOf<User, UsernameIsTaken, Error> CreateUser(string username, string password);

        public OneOf<Package, NotFound, Error> AcquirePackage(string username);

        public OneOf<List<Card>, NotFound, CardNotOwned, Error> SetDeck(string username, List<Guid> cardIds);
    }
}