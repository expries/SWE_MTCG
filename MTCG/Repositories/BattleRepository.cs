using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public class BattleRepository : IBattleRepository
    {
        private readonly Dictionary<Guid, Battle> _queue;

        public BattleRepository()
        {
            _queue = new Dictionary<Guid, Battle>();
        }

        public void DeleteGame(Battle battle)
        {
            var storedBattle = _queue.FirstOrDefault(x => x.Value.Equals(battle));
            _queue.Remove(storedBattle.Key);
        }
        
        public Battle FindGame(User user)
        {
            if (_queue.Count == 0)
            {
                return null;
            }
            
            int minEloDifference = _queue.Values.Min(x => x.PlayerA.Elo - user.Elo);
            var battle = _queue.Values.First(x => x.PlayerA.Elo - user.Elo == minEloDifference);
            return battle;
        }

        public Battle CreateGame(Battle battle)
        {
            if (_queue.ContainsKey(battle.PlayerA.Id))
            {
                return null;
            }
            
            _queue.Add(battle.PlayerA.Id, battle);
            return battle;
        }
    }
}