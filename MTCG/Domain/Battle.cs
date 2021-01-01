using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Battle
    {
        public User PlayerA { get; private set; }
        
        public User PlayerB { get; private set; }

        public bool Done;

        public BattleData MetaInfo;
        
        private Battle(User playerA)
        {
            PlayerA = playerA;
            PlayerB = null;
            Done = false;
            MetaInfo = new BattleData();
        }

        public static Result<Battle> Create(User playerA)
        {
            if (playerA is null)
            {
                return new MissingPlayer("A player is required to host a game.");
            }

            return new Battle(playerA);
        }

        public Result RegisterAsSecondPlayer(User playerB)
        {
            if (playerB is null)
            {
                return new MissingPlayer("A second player is required to play a game");
            }
            
            PlayerB = playerB;
            return Result.Ok();
        }

        private void Reset()
        {
            Done = false;
            MetaInfo = new BattleData();
        }

        public Result Play()
        {
            Reset();
            
            if (PlayerB is null)
            {
                return new MissingPlayer("A second player is required to play a game");
            }

            if (PlayerA.Deck.Count != 5)
            {
                return new DeckIsNotFull(PlayerA.Username + "  has an incomplete deck, can't play.");
            }
            
            if (PlayerB.Deck.Count != 5)
            {
                return new DeckIsNotFull(PlayerB.Username + " has an incomplete deck, can't play.");
            }

            var teamA = PlayerA.Deck;
            var teamB = PlayerB.Deck;
            int round = 0;
            
            while (teamA.Count > 0 && teamB.Count > 0 && round < 100)
            {
                var cardA = GetNextCard(teamA);
                var cardB = GetNextCard(teamB);
                var battle = BattleLogic.Fight(cardA, cardB, PlayerA.Username, PlayerB.Username);
                string battleLog = battle.ToString();
                TransferCards(battle.WinningCard, cardA, cardB, teamA, teamB);
                MetaInfo.Log(battleLog);
                round++;
            }

            if (teamA.Count > teamB.Count)
            {
                MetaInfo.AddWinner(PlayerA);
            }
            
            if (teamB.Count > teamA.Count)
            {
                MetaInfo.AddWinner(PlayerB);
            }

            Done = true;
            return Result.Ok();
        }

        private static void TransferCards(Card winner, Card cardA, Card cardB, List<Card> teamA, List<Card> teamB)
        {
            if (winner == cardA)
            {
                teamB.Remove(cardB);
                teamA.Add(cardB);
            }

            if (winner == cardB)
            {
                teamA.Remove(cardA);
                teamB.Add(cardA);
            }
        }

        private static Card GetNextCard(List<Card> cards)
        {
            int index = new Random().Next(cards.Count);
            var card = cards[index];
            return card;
        }
    }
}