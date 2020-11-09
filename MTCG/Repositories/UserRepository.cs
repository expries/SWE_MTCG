using System;
using System.Collections.Generic;
using MTCG.Resources;

namespace MTCG.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users;
        private int _userCounter;
        
        public UserRepository()
        {
            _users = new Dictionary<string, User>();
            _userCounter = 0;
        }

        public User CreateUser(string username, string password)
        {
            if (_users.ContainsKey(username))
            {
                return null;
            }
            
            var user = new User(username, password);
            user.Id = ++_userCounter;
            user.Token = Guid.NewGuid().ToString();
            
            _users.Add(user.Username, user);
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
    }
}