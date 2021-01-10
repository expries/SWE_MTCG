using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Server;

namespace MTCG.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;
        private readonly ITradeRepository _tradeRepository;

        public UserController(IUserRepository userRepository, IPackageRepository packageRepository, 
            ICardRepository cardRepository, ITradeRepository tradeRepository)
        {
            _userRepository = userRepository;
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
            _tradeRepository = tradeRepository;
        }

        public ResponseContext RegisterUser(RegistrationRequest request)
        {
            var user = _userRepository.GetByUsername(request.Username);

            if (user != null)
            {
                return Conflict("This username is taken.");
            }

            var userCreation = User.Create(request.Username,request.Password);

            if (!userCreation.Success)
            {
                return BadRequest(userCreation.Error);
            }

            user = userCreation.Value;
            user.AddCoins(20);
            var newUser = _userRepository.Create(user);
            
            return Created(newUser);
        }

        public ResponseContext UpdateUser(string token, string username, UserUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }
            
            if (user.Username != username)
            {
                return Forbidden("You are not permitted to alter this user's profile.");
            }
            
            var changeName = user.ChangeName(request.Name);

            if (!changeName.Success)
            {
                return BadRequest(changeName.Error);
            }

            user.Bio = request.Bio;
            user.Image = request.Image;
            _userRepository.Update(user);
            
            return NoContent();
        }

        public ResponseContext GetUser(string token, string username)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }
            
            var user = _userRepository.GetByUsername(username);

            if (user is null)
            {
                return NotFound("No user with this username exists.");
            }
            
            if (user.Token != token)
            {
                return Forbidden("You are not permitted to to view this user's profile.");
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
                return Unauthorized("No user with this username and password exists.");
            }

            return Ok(user.Token);
        }

        public ResponseContext GetStack(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }
            
            return Ok(user.Stack);
        }

        public ResponseContext AcquirePackage(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }

            var packages = _packageRepository.GetAll();
            var package = packages.FirstOrDefault();

            if (package is null)
            {
                return Conflict("There are no more packages.");
            }

            var purchase = user.PurchasePackage(package);

            if (!purchase.Success)
            {
                return Conflict(purchase.Error);
            }

            _userRepository.Update(user);
            _packageRepository.Delete(package);
            return Ok(package);
        }

        public ResponseContext GetDeck(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }
            
            return Ok(user.Deck);
        }

        public ResponseContext UpdateDeck(string token, List<Guid> cardIds)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }

            var cardsNotFound = cardIds.Where(x => _cardRepository.Get(x) is null).ToList();

            if (cardsNotFound.Any())
            {
                return NotFound("Can't update deck, These cards don't exist: " + 
                                string.Join(", ", cardsNotFound));
            }

            var tradeCards = _tradeRepository.GetForUser(user).Select(x => x.CardToTrade.Id);
            var cardsInTrade = cardIds.Where(x => tradeCards.Contains(x)).ToList();

            if (cardsInTrade.Any())
            {
                return Conflict("Can't update deck, these cards are currently in the user's deck: " +
                                string.Join(", ", cardsInTrade));
            }

            var cards = cardIds.Select(id => _cardRepository.Get(id)).ToList();
            var setDeck = user.SetDeck(cards);

            if (!setDeck.Success)
            {
                return Conflict(setDeck.Error);
            }
            
            _userRepository.Update(user);
            return NoContent();
        }

        public ResponseContext GetDeckPlaintext(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized("Authorization is required.");
            }
            
            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication failed with provided token.");
            }

            string output = string.Empty;

            foreach (var card in user.Deck)
            {
                output += "Id=" + card.Id + ";" + 
                          "Name=" + card.Name + ";" + 
                          "Type=" + card.Type + ";" + 
                          "Element=" + card.Element + ";" + 
                          "Damage=" + card.Damage + "\n";
            }
            
            return Ok(output, MediaType.Plaintext);
        }
    }
}