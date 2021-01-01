using System.Collections.Generic;
using MTCG.Entities;
using MTCG.Repositories;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly IUserRepository _userRepository;

        public ScoreService(IScoreRepository scoreRepository, IUserRepository userRepository)
        {
            _scoreRepository = scoreRepository;
            _userRepository = userRepository;
        }
        
        public Result<StatsEntity> GetStats(string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new NotPermitted("You have to provide a your token to access your stats.");
            }

            var stats = _scoreRepository.GetScoreForUser(user.Username);

            if (stats is null)
            {
                return new StatsNotFound("No score for the user " + user.Username + " exists.");
            }

            return stats;
        }

        public Result<List<StatsEntity>> GetScoreboard(string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new NotPermitted("You have to provide a valid token to access the scoreboards.");
            }

            return _scoreRepository.GetScoreboard();
        }
    }
}