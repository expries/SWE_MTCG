using System.Collections.Generic;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public interface IScoreRepository
    {
        public StatsEntity GetScoreForUser(string username);

        public List<StatsEntity> GetScoreboard();
    }
}