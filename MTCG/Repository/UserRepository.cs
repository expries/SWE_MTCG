using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Resource;

namespace MTCG.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users;
        private readonly Dictionary<string, List<Guid>> _packages;

        public UserRepository()
        {
            _users = new Dictionary<string, User>();
            _packages = new Dictionary<string, List<Guid>>();
        }

        public User CreateUser(string username, string password)
        {
            var user = new User(username, password, Guid.NewGuid());
            user.AddCoins(5);
            _users.Add(user.Username, user);
            _packages.Add(user.Username, new List<Guid>());
            return user;
        }

        public bool CheckCredentials(string username, string password)
        {
            if (!_users.ContainsKey(username))
            {
                return false;
            }
            return _users[username].Password == password;
        }

        public User GetUser(string username)
        {
            return _users.ContainsKey(username) ? _users[username] : null;
        }

        public List<User> GetAllUsers()
        {
            return _users.Values.ToList();
        }

        public bool AcquirePackage(string username, Guid packageId)
        {
            if (!_packages.ContainsKey(username))
            {
                return false;
            }

            if (_packages[username].Contains(packageId))
            {
                return false;
            }

            _packages[username].Add(packageId);
            return true;
        }

        public List<Guid> GetPackageIds(string username)
        {
            return _packages.ContainsKey(username) ? _packages[username] : null;
        }

        public void AddCoins(string username, int coins)
        {
            var user = GetUser(username);
            user?.AddCoins(coins);
        }
    }
}