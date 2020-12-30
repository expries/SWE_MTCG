using MTCG.Repositories;
using MTCG.Server;
using MTCG.Services;

namespace MTCG.Controller
{
    public class StatsController : ApiController
    {
        private readonly IScoreService _scoreService;

        public StatsController(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        public ResponseContext GetStats(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }

            var result = _scoreService.GetStats(token);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }

            var stats = result.Value;
            return Ok(stats);
        }

        public ResponseContext GetScoreboard(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }
            
            var result = _scoreService.GetScoreboard(token);

            if (!result.Success)
            {
                return Conflict(result.Error);
            }

            var stats = result.Value;
            return Ok(stats);
        }
    }
}