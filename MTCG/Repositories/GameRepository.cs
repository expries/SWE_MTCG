using System;
using System.Collections.Concurrent;
using System.Linq;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, Game> _games;

        public GameRepository()
        {
            _games = new ConcurrentDictionary<Guid, Game>();
        }

        public void Delete(Game game)
        {
            var storedGame = _games.FirstOrDefault(x => x.Value.Equals(game));
            _games.TryRemove(storedGame);
        }
        
        public Game FindForUser(User user)
        {
            var games = _games.Values;
            var orderedGames = games.OrderBy(x => Math.Abs(user.Elo - x.PlayerA.Elo));
            var openGame = orderedGames.FirstOrDefault(x => x.IsOpen);
            return openGame;
        }

        public Game Create(Game game)
        {
            if (_games.ContainsKey(game.PlayerA.Id))
            {
                return null;
            }
        
            _games[game.PlayerA.Id] = game;
            return game;
        }
    }
}