using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public interface IStatsRepository
    {
        public Stats GetForUser(User user);

        public List<Stats> GetAll();
    }
}