using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Server;

namespace MTCG.Controller
{
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;

        public UserController(IUserRepository userRepository, IPackageRepository packageRepository, ICardRepository cardRepository)
        {
            _userRepository = userRepository;
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
        }

        public ResponseContext RegisterUser(RegistrationRequest request)
        {
            var user = _userRepository.GetByUsername(request.Username);

            if (user != null)
            {
                return Conflict(new {Error = "This username is taken."});
            }

            var userCreation = User.Create(request.Username,request.Password);

            if (!userCreation.Success)
            {
                return BadRequest(userCreation.Error);
            }

            user = userCreation.Value;
            user.AddCoins(10);
            var newUser = _userRepository.Create(user);
            
            return Ok(newUser.Token);
        }

        public ResponseContext UpdateUser(string token, string username, UserUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }
            
            if (user.Username != username)
            {
                return Forbidden(new {Error = "You are not permitted to alter this user's profile."});
            }
            
            var changeName = user.ChangeName(request.Name);

            if (!changeName.Success)
            {
                return BadRequest(changeName.Error);
            }

            user.Bio = request.Bio;
            user.Image = request.Image;
            var updatedUser = _userRepository.Update(user);
            
            return Ok(updatedUser);
        }

        public ResponseContext GetUser(string username)
        {
            var user = _userRepository.GetByUsername(username);

            if (user is null)
            {
                return NotFound(new {Error = "No user with this username exists."});
            }
            
            return Ok(user);
        }

        public ResponseContext GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return Ok(users);
        }

        public ResponseContext Login(LoginRequest request)
        {
            var user = _userRepository.GetByUsername(request.Username);

            if (user is null || !user.ComparePassword(request.Password))
            {
                return Forbidden(new {Error = "No user with this username and password exists."});
            }

            return Ok();
        }

        public ResponseContext GetStack(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "Authentication failed with provided token."});
            }
            
            return Ok(user.Stack);
        }

        public ResponseContext AcquirePackage(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "Authentication failed with provided token."});
            }

            var package = GetUnownedPackage(user);

            if (package is null)
            {
                return Conflict(new {Error = "User already owns all packages"});
            }

            var purchase = user.PurchasePackage(package);

            if (!purchase.Success)
            {
                return Conflict(purchase.Error);
            }

            _userRepository.Update(user);
            return Ok(package);
        }

        public ResponseContext GetDeck(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "Authentication failed with provided token."});
            }
            
            return Ok(user.Deck);
        }

        public ResponseContext UpdateDeck(string token, List<Guid> cardIds)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "Authentication failed with provided token."});
            }

            var cardsNotFound = cardIds.Where(x => _cardRepository.Get(x) is null).ToList();

            if (cardsNotFound.Any())
            {
                return NotFound(new 
                    {Error = "These cards don't exist: " + string.Join(", ", cardsNotFound)});
            }

            var cards = cardIds.Select(id => _cardRepository.Get(id)).ToList();
            var setDeck = user.SetDeck(cards);

            if (!setDeck.Success)
            {
                return Conflict(setDeck.Error);
            }
            
            var updatedUser = _userRepository.Update(user);
            return Ok(updatedUser.Deck);
        }
        
        private Package GetUnownedPackage(User user)
        {
            var cardIds = user.Stack.Select(card => card.Id);
            var packages = _packageRepository.GetAll();
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