using System;
using System.Collections.Generic;
using MTCG.Resources;

namespace MTCG.Repositories
{
    public interface IUserRepository
    {
        public List<User> GetAllUsers();

        public User GetUser(string username);

        public User CreateUser(string username, string password);
        
        public bool CheckCredentials(string username, string password);

        public List<Guid> GetPackageIds(string username);

        public bool AcquirePackage(string username, Guid packageId);

        public void AddCoins(string username, int coins);
    }
}