using MTCG.Repositories;
using MTCG.Server;

namespace MTCG.Controller
{
    public class StatsController : ApiController
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly IUserRepository _userRepository;

        public StatsController(IScoreRepository scoreRepository, IUserRepository userRepository)
        {
            _scoreRepository = scoreRepository;
            _userRepository = userRepository;
        }

        public ResponseContext GetStats(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }

            var stats = _scoreRepository.GetByUsername(user.Username);

            if (stats is null)
            {
                return NotFound(new {Error = "Could not find stats for user \"" + user.Username + "\""});
            }
            
            return Ok(stats);
        }

        public ResponseContext GetScoreboard(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            if (_userRepository.GetByToken(token) is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }

            var scoreboard = _scoreRepository.GetAll();
            return Ok(scoreboard);
        }
    }
}