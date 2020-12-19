using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
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

        public OneOf<User,NotFound> GetUser(string username)
        {
            var user =  _userRepository.GetUser(username);
            
            if (user is null)
            {
                return new NotFound("User not found.");
            }

            return user;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public bool CheckCredentials(string username, string password)
        {
            var userResult = GetUser(username);
            
            return userResult.Match(
                user => CalculateSha256Hash(password) == user.Password,
                notFound => false);
        }
        
        public bool VerifyUser(string username)
        {
            var userResult = GetUser(username);
            return userResult.Match(user => true, notFound => false);
        }

        public OneOf<Package, NotFound, Error> AcquirePackage(string username)
        {
            var userResult = GetUser(username);

            if (userResult.IsT2())
            {
                return new NotFound();
            }

            var user = userResult.GetT1(); 
            var package = GetUnownedPackage(user);

            if (package is null)
            {
                return new AllPackagesAcquired();
            }
            
            if (user.Coins < package.Price)
            {
                return new NotEnoughCoins();
            }

            _userRepository.AddPackageToUser(username, package);
            _userRepository.AddCoins(username, (-1) * package.Price);
            return package;
        }

        public OneOf<User, UsernameIsTaken, Error> CreateUser(string username, string password)
        {
            var userResult = GetUser(username);

            if (userResult.IsT1())
            {
                return new UsernameIsTaken();
            }

            var user = userResult.GetT1();

            try
            {
                string passwordHash = CalculateSha256Hash(password);
                user = new User(username, passwordHash, Guid.NewGuid());
                user.AddCoins(10);
            }
            catch (ArgumentException error)
            {
                return new BadFormat(error.Message);
            }

            var createdUser = _userRepository.CreateUser(user);
            return createdUser;
        }

        public OneOf<List<Card>, NotFound, CardNotOwned, Error> SetDeck(string username, List<Guid> cardIds)
        {
            var userResult = GetUser(username);
            
            if (userResult.IsT2())
            {
                return new NotFound();
            }

            var user = userResult.GetT1();

            var nonExistentCards = cardIds
                .Where(cardId => _cardService.GetCard(cardId) is null)
                .ToList();

            if (nonExistentCards.Any())
            {
                string message = "These cards don't exist: " + string.Join(", ", nonExistentCards);
                return new NotFound(message);
            }

            var userCards = user.Stack.Select(card => card.Id);
            var unownedCards = cardIds.Where(id => !userCards.Contains(id)).ToList();

            if (unownedCards.Any())
            {
                string message = "You do not own these cards: " + string.Join(", ", unownedCards) + ".";
                return new CardNotOwned(message);
            }

            _userRepository.EmptyDeck(username);
            _userRepository.SetDeck(username, cardIds);

            var deck = GetUser(username).GetT1().Deck;  // user exists if arrived here
            return deck;
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