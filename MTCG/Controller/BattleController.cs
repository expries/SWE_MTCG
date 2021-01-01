using System.Threading;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Server;

namespace MTCG.Controller
{
    public class BattleController : ApiController
    {
        private readonly IBattleRepository _battleRepository;
        private readonly IUserRepository _userRepository;

        public BattleController(IBattleRepository battleRepository, IUserRepository userRepository)
        {
            _battleRepository = battleRepository;
            _userRepository = userRepository;
        }

        public ResponseContext PlayGame(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }

            if (user.Deck.Count != 5)
            {
                return Conflict(new {Error = "Can't play: Deck does not consist of 5 cards."});
            }

            var battle = _battleRepository.FindGame(user);

            // found a game => play it (game host will clean up)
            if (battle != null)
            {
                battle.RegisterAsSecondPlayer(user);
                battle.Play();
                string battleLog = battle.MetaInfo.ToString();
                return Ok(battleLog);
            }

            // found no game => host your own
            var createBattle = Battle.Create(user);

            if (!createBattle.Success)
            {
                return BadRequest(createBattle.Error);
            }

            battle = _battleRepository.CreateGame(createBattle.Value);

            if (battle is null)
            {
                return Conflict("You are already queueing for a game.");
            }

            while (!battle.Done)
            {
                Thread.Sleep(100);
            }
            
            if (battle.MetaInfo.Winner == battle.PlayerA)
            {
                battle.PlayerA.AddWin();
                battle.PlayerB.AddLose();
            }
            else if (battle.MetaInfo.Winner == battle.PlayerB)
            {
                battle.PlayerA.AddLose();
                battle.PlayerB.AddWin();
            }
            else
            {
                battle.PlayerA.AddDraw();
                battle.PlayerB.AddDraw();
            }

            // done! update user stats and remove game
            _userRepository.UpdateUser(battle.PlayerA);
            _userRepository.UpdateUser(battle.PlayerB);
            _battleRepository.DeleteGame(battle);
            
            string gameLogs = battle.MetaInfo.ToString();
            return Ok(gameLogs);
        }
    }
}