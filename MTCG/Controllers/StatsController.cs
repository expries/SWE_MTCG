using MTCG.Repositories;
using MTCG.Server;

namespace MTCG.Controllers
{
    public class StatsController : ApiController
    {
        private readonly IStatsRepository _statsRepository;
        private readonly IUserRepository _userRepository;

        public StatsController(IStatsRepository statsRepository, IUserRepository userRepository)
        {
            _statsRepository = statsRepository;
            _userRepository = userRepository;
        }

        public ResponseContext GetStats(string token)
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

            var stats = _statsRepository.GetForUser(user);

            if (stats is null)
            {
                return NotFound("Could not find stats for user \"" + user.Username + "\"");
            }
            
            return Ok(stats);
        }

        public ResponseContext GetScoreboard(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            if (_userRepository.GetByToken(token) is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            var scoreboard = _statsRepository.GetAll();
            return Ok(scoreboard);
        }
    }
}