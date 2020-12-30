using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Domain.Cards;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;
        
        public UserService(IUserRepository userRepository, IPackageRepository packageRepository, ICardRepository cardRepository)
        {
            _userRepository = userRepository;
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
        }

        public Result<User> GetUser(string username)
        {
            var user =  _userRepository.GetUserByUsername(username);
            
            if (user is null)
            {
                return new UserNotFound("User not found.");
            }

            return user;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public bool CheckCredentials(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user is null)
            {
                return false;
            }
            
            return user.ComparePassword(password);
        }
        
        public Result<User> GetUserByAuthToken(string token)
        {
            var userResult = _userRepository.GetUserByToken(token);

            if (userResult is null)
            {
                return new UserNotFound("No user with such an authentication token exists.");
            }

            return userResult;
        }

        public Result<Package> AcquirePackage(string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new UserNotFound("There is no user with auth token " + token + " .");
            }
            
            var package = GetUnownedPackage(user);

            if (package is null)
            {
                return new AllPackagesAcquired("Sorry, you already own all the packages there are.");
            }
            
            if (user.Coins < package.Price)
            {
                return new NotEnoughCoins("Sorry, you don't have enough coins to purchase this package.");
            }

            user.AddCoins((-1) * package.Price);
            package.Cards.ForEach(card => user.AddToCollection(card));
            
            _userRepository.UpdateUser(user);
            return package;
        }

        public Result<User> CreateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user != null)
            {
                return new UsernameIsTaken("This username is taken.");
            }

            var userCreation = User.Create(username, password);

            if (!userCreation.Success)
            {
                return new BadUser(userCreation.Error.Message);
            }

            user = userCreation.Value;
            user.AddCoins(10);
            var createdUser = _userRepository.CreateUser(user);
            return createdUser;
        }

        public Result<User> UpdateUser(string username, UserUpdateRequest request, string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new UserNotFound("No user with this token exists.");
            }

            if (user.Username != username)
            {
                return new NotPermitted("You are not permitted to alter this user's profile.");
            }

            var changeName = user.SetName(request.Name);

            if (!changeName.Success)
            {
                return new BadUser(changeName.Error.Message);
            }

            user.Bio = request.Bio;
            user.Image = request.Image;
            var updatedUser = _userRepository.UpdateUser(user);
            return updatedUser;
        }

        public Result<List<Card>> UpdateDeck(string token, List<Guid> cardIds)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new UserNotFound("There is no user with auth token " + token + " .");
            }

            var cardsNotFound = cardIds.Where(id => _cardRepository.GetCard(id) is null).ToList();

            if (cardsNotFound.Any())
            {
                return new CardNotFound(
                    "These cards don't exist: " + string.Join(", ", cardsNotFound) + ".");
            }

            var userCards = user.Stack.Select(card => card.Id);
            var unownedCards = cardIds.Where(id => !userCards.Contains(id)).ToList();

            if (unownedCards.Any())
            {
                return new CardNotOwned(
                    "You do not own these cards: " + string.Join(", ", unownedCards) + ".");
            }

            var cards = cardIds.Select(id => _cardRepository.GetCard(id)).ToList();
            user.ClearDeck();
            cards.ForEach(card => user.AddToDeck(card));
            
            var updatedUser = _userRepository.UpdateUser(user);
            return updatedUser.Deck;
        }

        private Package GetUnownedPackage(User user)
        {
            var cardIds = user.Stack.Select(card => card.Id);
            var packages = _packageRepository.GetAllPackages();
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
    }
}