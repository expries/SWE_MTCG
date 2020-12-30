using System;
using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Entities;
using MTCG.Results;

namespace MTCG.Repositories
{
    public interface IUserRepository
    {
        public User GetUserByUsername(string username);

        public User GetUserByToken(string token);

        public User GetUserById(Guid userId);
        
        public List<User> GetAllUsers();

        public User CreateUser(User user);
        
        public User UpdateUser(User user);
    }
}