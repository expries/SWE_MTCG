using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public class StatsRepository : IStatsRepository
    {
        private readonly DatabaseManager _db;
        
        public StatsRepository(DatabaseManager db)
        {
            _db = db;
        }
        
        public Stats GetForUser(User user)
        {
            const string sql = "SELECT rank, username, elo, total, wins, loses, draws, winRate " +
                               "FROM scoreboard WHERE username = @username";

            var entity = _db.QueryFirstOrDefault<StatsEntity>(sql, new {Username = user.Username});
            var stats = MapEntity(entity);
            return stats;
        }

        public List<Stats> GetAll()
        {
            const string sql = "SELECT rank, username, elo, total, wins, loses, draws, winRate " +
                               "FROM scoreboard";

            var entities = _db.Query<StatsEntity>(sql);
            var stats = entities.Select(MapEntity).ToList();
            return stats;
        }

        private Stats MapEntity(StatsEntity entity)
        {
            var createStats = 
                Stats.Create(entity.Rank, entity.Username, entity.Elo, entity.Wins, entity.Loses, entity.Draws);

            return createStats.Value;
        }
    }
}