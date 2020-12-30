using System;
using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Entities;
using MTCG.Requests;
using MTCG.Results;

namespace MTCG.Services
{
    public interface IUserService
    {
        public Result<User> GetUser(string username);

        public List<User> GetAllUsers();

        public Result<User> UpdateUser(string username, UserUpdateRequest request, string token);

        public bool CheckCredentials(string username, string password);
        
        public Result<User> GetUserByAuthToken(string token);

        public Result<User> CreateUser(string username, string password);

        public Result<Package> AcquirePackage(string token);

        public Result<List<Card>> UpdateDeck(string token, List<Guid> cardIds);
    }
}