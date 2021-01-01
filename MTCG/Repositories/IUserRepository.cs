using System;
using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Entities;
using MTCG.Results;

namespace MTCG.Repositories
{
    public interface IUserRepository
    {
        public User GetByUsername(string username);

        public User GetByToken(string token);

        public User GetById(Guid userId);
        
        public List<User> GetAll();

        public User Create(User user);
        
        public User Update(User user);
    }
}