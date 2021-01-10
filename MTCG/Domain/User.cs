using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;
using Newtonsoft.Json;

namespace MTCG.Domain
{
    public class User
    {
        [JsonIgnore]
        public Guid Id { get; }

        public string Username { get; }
        
        [JsonIgnore]
        public string Password { get; private set; }
        
        public string Name { get; private set; }
        
        public string Bio { get; set; }
        
        public string Image { get; set; }
        
        public int Coins { get; private set; }

        [JsonIgnore]
        public string Token => GetToken();
        
        [JsonIgnore]
        public int Wins { get; private set; }
        
        [JsonIgnore]
        public int Draws { get; private set; }
        
        [JsonIgnore]
        public int Loses { get; private set; }
        
        [JsonIgnore]
        public int Elo { get; private set; }

        [JsonIgnore]
        public readonly List<Card> Stack;
        
        [JsonIgnore]
        public List<Card> Deck => GetDeck();
        
        private readonly List<Guid> _deckIds;

        private const int EloSensitivity = 40;

        private User(string username, string password, Guid id, int coins, string name, string bio, string image, 
            int wins, int draws, int loses, int elo)
        {
            Id = id;
            Username = username;
            Password = GetSha256Hash(password);
            Bio = bio;
            Name = name;
            Image = image;
            
            Wins = wins;
            Draws = draws;
            Loses = loses;
            Elo = elo;
            Coins = coins;
            Stack = new List<Card>();
            _deckIds = new List<Guid>();
        }

        public static Result<User> Create(string username, string password, Guid id, int coins = 0, string name = null, 
            string bio = null, string image = null, int wins = 0, int loses = 0, int draws = 0, int elo = 0)
        {
            if (id.Equals(Guid.Empty))
            {
                return new CardIdIsEmpty("User ID may not be empty.");
            }

            if (wins < 0)
            {
                return new NegativeWins("Wins may not be negative.");
            }
            
            if (draws < 0)
            {
                return new NegativeDraws("Draws may not be negative.");
            }
            
            if (loses < 0)
            {
                return new NegativeLoses("Loses may not be negative.");
            }
            
            if (elo < 0)
            {
                return new NegativeElo("Elo may not be negative.");
            }

            if (coins < 0)
            {
                return new NegativeCoins("Coins may not be negative.");
            }
            
            if (string.IsNullOrWhiteSpace(username))
            {
                return new UsernameIsEmpty("Username may not be empty.");
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                return new PasswordIsEmpty("Password may not be empty.");
            }

            return new User(username, password, id, coins, name, bio, image, wins, draws, loses, elo);
        }

        public static Result<User> Create(string username, string password, int coins = 0, string name = null, 
            string bio = null, string image = null, int wins = 0, int draws = 0, int loses = 0, int elo = 0)
        {
            var id = Guid.NewGuid();
            return Create(username, password, id, coins, name, bio, image, wins, loses, draws, elo);
        }

        public Result ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new NameIsEmpty("Name may not be empty.");
            }

            Name = name;
            return Result.Ok();
        }

        public Result SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return new PasswordIsEmpty("Password hash may not be empty.");
            }

            Password = passwordHash;
            return Result.Ok();
        }

        public void AddCoins(int coins)
        {
            Coins = Math.Max(0, Coins + coins);
        }

        public void AddWinAgainst(User opponent)
        {
            Elo = CalculateNewElo(opponent, 1);
            Wins++;
        }

        public void AddLoseAgainst(User opponent)
        {
            Elo = CalculateNewElo(opponent, 0);
            Loses++;
        }
        
        public void AddDrawAgainst(User opponent)
        {
            Elo = CalculateNewElo(opponent, 0.5);
            Draws++;
        }

        public Result AddToStack(Card card)
        {
            if (Stack.Any(x => x.Id == card.Id))
            {
                return new CardAlreadyInStack("Card with id " + card.Id + " is already in the stack.");
            }
            
            Stack.Add(card);
            return Result.Ok();
        }

        public Result RemoveFromStack(Card card)
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

        public bool DeckIsComplete()
        {
            return _deckIds.Count == 4;
        }

        public Result SetDeck(List<Card> cards)
        {
            if (cards.Any(x => x is null))
            {
                return new CardIsNull("Some cards are null");
            }

            if (cards.Count != 4)
            {
                return new DeckIsNotFull("Deck does not consist of 4 cards");
            }

            var stackIds = Stack.Select(x => x.Id);
            var unownedCards = cards.Select(x => x.Id).Where(x => !stackIds.Contains(x)).ToList();

            if (unownedCards.Any())
            {
                return new CardNotInStack(
                    "These cards are not in the stack: " + string.Join(",", unownedCards));
            }
            
            ClearDeck();
            cards.ForEach(x => AddToDeck(x));
            return Result.Ok();
        }

        public Result PurchasePackage(Package package)
        {
            if (package is null)
            {
                return new PackageIsNull("Package is required for a user to acquire it.");
            }

            if (package.Price > Coins)
            {
                return new NotEnoughCoins("The user does not own enough coins to purchase the package.");
            }
            
            package.Cards.ForEach(card => AddToStack(card));
            Coins -= package.Price;
            return Result.Ok();
        }
        
        public bool OwnsPackage(Package package)
        {
            return package is not null && package.Cards.Any(x => Stack.Any(y => y.Id == x.Id));
        }

        public bool ComparePassword(string password)
        {
            return GetSha256Hash(password) == Password;
        }

        private int CalculateNewElo(User opponent, double score)
        {
            double expectedInverse = 1 + Math.Pow(10, (opponent.Elo - Elo) / 400);
            double expected = 1 / expectedInverse;
            double difference = EloSensitivity * (score - expected);
            return Convert.ToInt32(Elo + difference);
        }
        
        private Result AddToDeck(Card card)
        {
            if (_deckIds.Contains(card.Id))
            {
                return new CardAlreadyInDeck("This card is already in the deck.");
            }

            if (_deckIds.Count > 4)
            {
                return new DeckIsFull("There are already 4 cards in the stack.");
            }

            if (Stack.All(x => x.Id != card.Id))
            {
                return new CardNotInStack("There is no card in the stack with ID " + card.Id +  ".");
            }

            _deckIds.Add(card.Id);
            return Result.Ok();
        }

        private string GetToken()
        {
            return $"{Username}-mtcgToken";
        }

        private List<Card> GetDeck()
        {
            return Stack.Where(card => _deckIds.Contains(card.Id)).ToList();
        }

        private static string GetSha256Hash(string input)
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