using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class User
    {
        public Guid Id { get; }

        public string Username { get; }
        
        public string Password { get; }
        
        public string Name { get; private set; }
        
        public string Bio { get; set; }
        
        public string Image { get; set; }
        
        public int Coins { get; private set; }

        public string Token => GetToken();
        
        public int Wins { get; private set; }
        
        public int Draws { get; private set; }
        
        public int Loses { get; private set; }
        
        public int Elo { get; private set; }

        public readonly List<Card> Stack;
        
        public List<Card> Deck => GetDeck();
        
        private readonly List<Guid> _deckIds;

        private User(string username, string password, Guid id, string name, string bio, string image)
        {
            Id = id;
            Username = username;
            Password = CalculateSha256Hash(password);
            Bio = bio;
            Name = name;
            Image = image;
            
            Wins = 0;
            Draws = 0;
            Loses = 0;
            Elo = 0;
            Coins = 0;
            Stack = new List<Card>();
            _deckIds = new List<Guid>();
        }

        public static Result<User> Create(string username, string password, Guid id, string name = null, 
            string bio = null, string image = null)
        {
            if (id.Equals(Guid.Empty))
            {
                return new CardIdIsEmpty("User ID may not be empty.");
            }
            
            if (string.IsNullOrWhiteSpace(username))
            {
                return new UsernameIsEmpty("Username may not be empty.");
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                return new PasswordIsEmpty("Password may not be empty.");
            }

            return new User(username, password, id, name, bio, image);
        }

        public static Result<User> Create(string username, string password, string name = null, string bio = null, 
            string image = null)
        {
            var id = Guid.NewGuid();
            return Create(username, password, id, name, bio, image);
        }

        public Result SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new NameIsEmpty("Name may not be empty.");
            }

            Name = name;
            return Result.Ok();
        }

        public void AddCoins(int coins)
        {
            Coins = Math.Max(0, Coins + coins);
        }

        public Result AddToCollection(Card card)
        {
            if (Stack.Select(x => x.Id).Contains(card.Id))
            {
                return new CardAlreadyInStack("Card with id " + card.Id + " is already in the stack.");
            }
            
            Stack.Add(card);
            return Result.Ok();
        }

        public Result RemoveFromCollection(Card card)
        {
            var cardInStack = Stack.FirstOrDefault(x => x.Id == card.Id);

            if (cardInStack is null)
            {
                return new CardNotInStack("Card with id " + card.Id + " is not in the stack.");
            }

            if (_deckIds.Contains(card.Id))
            {
                _deckIds.Remove(card.Id);
            }
            
            Stack.Remove(cardInStack);
            return Result.Ok();
        }

        public void ClearDeck()
        {
            _deckIds.Clear();
        }

        public Result AddToDeck(Card card)
        {
            if (_deckIds.Contains(card.Id))
            {
                return new CardAlreadyInDeck("This card is already in the deck.");
            }

            if (_deckIds.Count > 5)
            {
                return new DeckIsFull("There are already 5 cards in the stack.");
            }

            if (!Stack.Select(x => x.Id).Contains(card.Id))
            {
                return new CardNotInStack("There is no card in the stack with ID " + card.Id +  ".");
            }

            _deckIds.Add(card.Id);
            return Result.Ok();
        }

        public Result RemoveFromDeck(Card card)
        {
            if (!_deckIds.Contains(card.Id))
            {
                return new CardNotInDeck("There is no card in the deck with ID " + card.Id + ".");
            }
            
            _deckIds.Remove(card.Id);
            return Result.Ok();
        }
        
        public bool ComparePassword(string password)
        {
            return CalculateSha256Hash(password) == Password;
        }

        private string GetToken()
        {
            return $"{Username}-mtcgToken";
        }

        private List<Card> GetDeck()
        {
            return Stack.Where(card => _deckIds.Contains(card.Id)).ToList();
        }
        
        private static string CalculateSha256Hash(string input)
        {
            using var hasher = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = hasher.ComputeHash(inputBytes);
            var builder = new StringBuilder();  
                
            foreach (byte t in hashBytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}