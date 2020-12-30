using System.Collections.Generic;
using MTCG.Database;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly DatabaseManager _db;
        
        public ScoreRepository(DatabaseManager db)
        {
            _db = db;
        }
        
        public StatsEntity GetScoreForUser(string username)
        {
            const string sql = "SELECT rank, username, elo, total, wins, loses, draws, winRate " +
                               "FROM scoreboard WHERE username = @username";

            var entity = _db.QueryFirstOrDefault<StatsEntity>(sql, new {Username = username});
            return entity;
        }

        public List<StatsEntity> GetScoreboard()
        {
            const string sql = "SELECT rank, username, elo, total, wins, loses, draws, winRate " +
                               "FROM scoreboard";

            var entities = _db.Query<StatsEntity>(sql);
            return entities;
        }
    }
}