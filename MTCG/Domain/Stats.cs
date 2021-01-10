using System;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Stats
    {
        public long Rank { get; }
        
        public string Username { get; }
        
        public double Elo { get; }
        
        public int Wins { get; }
        
        public int Loses { get; }
        
        public int Draws { get; }
        
        public int Total { get; }
        
        public double WinRate { get; }

        private Stats(long rank, string username, double elo, int wins, int loses, int draws, int total, double winRate)
        {
            Rank = rank;
            Username = username;
            Elo = elo;
            Wins = wins;
            Loses = loses;
            Draws = draws;
            Total = total;
            WinRate = winRate;
        }

        public static Result<Stats> Create(long rank, string username, double elo, int wins, int loses, int draws)
        {
            if (rank <= 0)
            {
                return new NegativeRank("Rank may not be negative.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return new UsernameIsEmpty("Username is required.");
            }

            if (elo < 0)
            {
                return new NegativeElo("Elo may not be negative.");
            }

            if (wins < 0)
            {
                return new NegativeWins("Wins may not be negative.");
            }

            if (loses < 0)
            {
                return new NegativeLoses("Loses may not be negative.");
            }

            if (draws < 0)
            {
                return new NegativeDraws("Draws may not be negative.");
            }

            int total = wins + loses + draws;
            double winRate = wins / Math.Max(1, total);
            return new Stats(rank, username, elo, wins, loses, draws, total, winRate);
        }
    }
}