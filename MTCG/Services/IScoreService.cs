using System.Collections.Generic;
using MTCG.Entities;
using MTCG.Results;

namespace MTCG.Services
{
    public interface IScoreService
    {
        public Result<StatsEntity> GetStats(string token);
        
        public Result<List<StatsEntity>> GetScoreboard(string token);
    }
}