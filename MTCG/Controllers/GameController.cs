using System;
using System.Resources;
using System.Threading;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Server;

namespace MTCG.Controllers
{
    public class GameController : ApiController
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;

        public GameController(IGameRepository gameRepository, IUserRepository userRepository)
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
        }

        public ResponseContext PlayGame(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            if (!user.DeckIsComplete())
            {
                return Conflict("Can't play: Deck is not complete.");
            }

            Game game;

            lock (_gameRepository)
            {
                game = _gameRepository.FindForUser(user);

                // found a game => play it (game host will clean up)
                if (game != null)
                {
                    game.JoinLobby(user);
                    game.IsOpen = false;
                    string battleLog = game.Log.ToString();
                    return Ok(battleLog);
                }

                // found no game => host your own
                var createGame = Game.CreateLobby(user);

                if (!createGame.Success)
                {
                    return BadRequest(createGame.Error);
                }

                game = _gameRepository.Create(createGame.Value);
            }
            
            if (game is null)
            {
                return Conflict("You are already queueing for a game.");
            }

            while (!game.IsDone)
            {
                Thread.Sleep(100);
            }
            
            if (game.Log.Winner == game.PlayerA)
            {
                game.PlayerA.AddWinAgainst(game.PlayerB);
                game.PlayerB.AddLoseAgainst(game.PlayerA);
            }
            else if (game.Log.Winner == game.PlayerB)
            {
                game.PlayerA.AddLoseAgainst(game.PlayerB);
                game.PlayerB.AddWinAgainst(game.PlayerA);
            }
            else
            {
                game.PlayerA.AddDrawAgainst(game.PlayerB);
                game.PlayerB.AddDrawAgainst(game.PlayerA);
            }

            // done! update user stats and remove game
            _userRepository.Update(game.PlayerA);
            _userRepository.Update(game.PlayerB);
            _gameRepository.Delete(game);
            
            string gameLog = game.Log.ToString();
            return Ok(gameLog);
        }
    }
}