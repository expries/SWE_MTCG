using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MTCG.ActionResult;
using MTCG.Repository;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPackageService _packageService;
        private readonly ICardService _cardService;
        
        public UserService(IUserRepository userRepository, IPackageService packageService, ICardService cardService)
        {
            _userRepository = userRepository;
            _packageService = packageService;
            _cardService = cardService;
        }

        public User GetUser(string username)
        {
            return _userRepository.GetUser(username);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public bool VerifyLogin(string username, string password)
        {
            var user = _userRepository.GetUser(username);

            if (user is null)
            {
                return false;
            }

            return CalculateSha256Hash(password) == user.Password;
        }
        
        public bool VerifyUser(string username)
        {
            return GetUser(username) != null;
        }

        public ActionResult<Package> AcquirePackage(string username)
        {
            var user = GetUser(username);

            if (user is null)
            {
                return new ActionResult<Package>(ServiceError.UserNotFound, 
                    "No user with this username exists.");
            }

            var packageToAcquire = GetUnownedPackage(user);

            if (packageToAcquire is null)
            {
                return new ActionResult<Package>(ServiceError.AllPackagesAcquired, 
                    "The user already owns all the packages.");
            }
            
            if (user.Coins < packageToAcquire.Price)
            {
                return new ActionResult<Package>(ServiceError.NotEnoughCoins, 
                    "The user doesn't have enough coins to buy this package.");
            }

            _userRepository.AddPackageToUser(username, packageToAcquire);
            _userRepository.AddCoins(username, (-1) * packageToAcquire.Price);
            return new ActionResult<Package>(packageToAcquire);
        }

        public ActionResult<User> CreateUser(string username, string password)
        {
            var user = GetUser(username);

            if (user != null)
            {
                return new ActionResult<User>(ServiceError.UsernameIsTaken, "The username is taken.");
            }

            try
            {
                string passwordHash = CalculateSha256Hash(password);
                user = new User(username, passwordHash, Guid.NewGuid());
                user.AddCoins(10);
            }
            catch (ArgumentException error)
            {
                return new ActionResult<User>(ServiceError.BadFormat, error.Message);
            }

            var createdUser = _userRepository.CreateUser(user);
            return new ActionResult<User>(createdUser);
        }

        public List<Card> GetDeck(string username)
        {
            return GetUser(username) is null ? null : _userRepository.GetDeck(username);
        }

        public ActionResult<List<Card>> SetDeck(string username, List<Guid> cardIds)
        {
            var user = GetUser(username);
            
            if (user is null)
            {
                return new ActionResult<List<Card>>(ServiceError.UserNotFound, 
                    "No user with this username exists.");
            }

            var nonExistentCards = cardIds
                .Where(cardId => _cardService.GetCard(cardId) is null)
                .ToList();

            if (nonExistentCards.Any())
            {
                return new ActionResult<List<Card>>(ServiceError.CardNotFound, 
                    "These cards don't exist: " + string.Join(", ", nonExistentCards));
            }

            var userCards = user.Stack.Select(card => card.Id);
            var unownedCards = cardIds.Where(id => !userCards.Contains(id)).ToList();

            if (unownedCards.Any())
            {
                return new ActionResult<List<Card>>(ServiceError.CardNotOwned, 
                    "You do not own these cards: " + string.Join(", ", unownedCards) + ".");
            }

            _userRepository.EmptyDeck(username);
            bool success = _userRepository.SetDeck(username, cardIds);
            
            if (!success)
            {
                return new ActionResult<List<Card>>(ServiceError.DeckNotSet, "Deck could not be set.");
            }
            
            var deck = GetDeck(username);
            return new ActionResult<List<Card>>(deck);
        }

        private Package GetUnownedPackage(User user)
        {
            var cardIds = user.Stack.Select(card => card.Id);
            var packages = _packageService.GetAllPackages();
            var unownedPackages = new List<Package>();
            
            foreach (var package in packages)
            {
                var unownedCards = package.Cards.Where(card => !cardIds.Contains(card.Id));

                if (unownedCards.Any())
                {
                    unownedPackages.Add(package);
                }
            }

            if (!unownedPackages.Any())
            {
                return null;
            }

            int randomIndex = new Random().Next(unownedPackages.Count);
            return unownedPackages[randomIndex];
        }

        private static string CalculateSha256Hash(string input)
        {
            using var hasher = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = hasher.ComputeHash(inputBytes);
            var builder = new StringBuilder();  
                
            foreach (byte t in hashBytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}