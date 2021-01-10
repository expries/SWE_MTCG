using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Game
    {
        public User PlayerA { get; }
        
        public User PlayerB { get; private set; }

        public bool IsDone { get; private set; }
        
        public bool IsOpen { get; set; }

        public GameLog Log { get; private set; }
        
        private const int MaxRounds = 100;
        
        private List<Card> TeamA { get; }
        
        private List<Card> TeamB { get; }

        private Game(User host)
        {
            PlayerA = host;
            PlayerB = null;
            TeamA = new List<Card>();
            TeamB = new List<Card>();
            IsOpen = true;
            IsDone = false;
            Log = new GameLog();
        }

        public static Result<Game> CreateLobby(User host)
        {
            if (host is null)
            {
                return new MissingPlayer("A player is required to host a game.");
            }

            return new Game(host);
        }

        public Result JoinLobby(User newPlayer)
        {
            if (newPlayer is null)
            {
                return new MissingPlayer("A second player is required to play a game");
            }
            
            if (!newPlayer.DeckIsComplete())
            {
                return new DeckIsNotFull(newPlayer.Username + " has an incomplete deck, can't play.");
            }

            PlayerB = newPlayer;

            Launch();
            IsDone = true;
            return Result.Ok();
        }

        private void ResetLobby()
        {
            IsDone = false;
            Log = new GameLog();
            TeamA.Clear();
            TeamB.Clear();
            PlayerA.Stack.ForEach(x => TeamA.Add(x));
            PlayerB.Stack.ForEach(x => TeamB.Add(x));
        }

        private void Launch()
        {
            ResetLobby();
            PlayGame();

            if (Log.Rounds == MaxRounds || TeamA.Count == TeamB.Count)
            {
                Log.Append("This game was a draw (reached " + Log.Rounds + " rounds).");
                return;
            }

            var winner = TeamA.Count > TeamB.Count ? PlayerA : PlayerB;
            Log.AddWinner(winner).Append(winner.Username + " has won the game after " + Log.Rounds + " rounds!");
        }

        private void PlayGame()
        {
            while (TeamA.Any() && TeamB.Any() && Log.Rounds < MaxRounds)
            {
                var round = PlayRound();
                string roundLog = round.ToString();
                Log.Append(roundLog);
                Log.IncrementRounds();
            }
        }

        private GameLog PlayRound()
        {
            var cardA = GetRandomCard(TeamA);
            var cardB = GetRandomCard(TeamB);
            var battle = BattleLogic.Fight(cardA, cardB, PlayerA.Username, PlayerB.Username);
            TransferCards(battle.WinningCard, cardA, cardB);
            return battle;
        }

        private void TransferCards(Card winner, Card cardA, Card cardB)
        {
            if (winner == cardA)
            {
                TeamB.Remove(cardB);
                TeamA.Add(cardB);
            }

            if (winner == cardB)
            {
                TeamA.Remove(cardA);
                TeamB.Add(cardA);
            }
        }

        private static Card GetRandomCard(List<Card> cards)
        {
            int index = new Random().Next(cards.Count);
            var card = cards[index];
            return card;
        }
    }
}