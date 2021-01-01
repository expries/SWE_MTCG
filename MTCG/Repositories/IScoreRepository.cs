using System.Collections.Generic;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public interface IScoreRepository
    {
        public StatsEntity GetByUsername(string username);

        public List<StatsEntity> GetAll();
    }
}