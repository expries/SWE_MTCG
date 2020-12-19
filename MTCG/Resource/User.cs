using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Resource.Cards;

namespace MTCG.Resource
{
    public class User
    {
        public Guid Id { get; }

        public string Username { get; }
        
        public string Password { get; }
        
        public int Coins { get; private set; }

        public string Token => GetToken();

        public readonly List<Card> Stack;
        
        public List<Card> Deck => GetDeck();
        
        private readonly List<Guid> _deckIds;


        public User(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username may not be empty.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password may not be empty.");
            }

            Username = username;
            Password = password;
            Stack = new List<Card>();
            _deckIds = new List<Guid>();
            Coins = 0;
        }

        public User(string username, string password, Guid id) : this(username, password)
        {
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("User ID may not be empty.");
            }
            
            Id = id;
        }

        public void AddCoins(int coins)
        {
            Coins = Math.Max(0, Coins + coins);
        }

        public void AddCard(Card card)
        {
            if (card.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Card ID may not be empty.");
            }
            
            Stack.Add(card);
        }

        public void AddCardToDeck(Guid cardId)
        {
            if (_deckIds.Contains(cardId))
            {
                throw new ArgumentException("This card is already in the deck.");
            }

            if (_deckIds.Count > 5)
            {
                throw new ArgumentException("There are already 5 cards in the stack.");
            }
            
            var stackCardIds = Stack.Select(card => card.Id);
            
            if (!stackCardIds.Contains(cardId))
            {
                throw new ArgumentException("There is no card in the stack with ID " + cardId +  ".");
            }

            _deckIds.Add(cardId);
        }

        public void RemoveFromDeck(Guid cardId)
        {
            if (!_deckIds.Contains(cardId))
            {
                throw new ArgumentException("There is no card in the deck with ID " + cardId + ".");
            }
            
            _deckIds.Remove(cardId);
        }

        private string GetToken()
        {
            return $"{Username}-mtcgToken";
        }

        private List<Card> GetDeck()
        {
            return Stack.Where(card => _deckIds.Contains(card.Id)).ToList();
        }
    }
}