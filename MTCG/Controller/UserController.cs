using System.Collections.Generic;
using System.Linq;
using MTCG.Exception;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Resource;
using MTCG.Resource.Cards;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controller
{
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IPackageRepository _packageRepository;

        public UserController()
        {
            _userRepository = new UserRepository();
            _cardRepository = new CardRepository();
            _packageRepository = new PackageRepository();
        }

        public UserController(IUserRepository userRepository, 
                              ICardRepository cardRepository, 
                              IPackageRepository packageRepository)
        {
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _packageRepository = packageRepository;
        }

        public ResponseContext Register(RegistrationRequest request)
        {
            if (_userRepository.GetUser(request.Username) != null)
            {
                return Conflict("Username is taken.");
            }

            var user = _userRepository.CreateUser(request.Username, request.Password);
            return Created(user.Token.ToString());
        }

        public ResponseContext Get(string username)
        {
            var user = _userRepository.GetUser(username);

            return user == null 
                ? NotFound("User not found.") 
                : Ok(user);
        }

        public ResponseContext GetAll()
        {
            var users = _userRepository.GetAllUsers();
            return Ok(users);
        }
        
        public ResponseContext Login(LoginRequest request)
        {
            return !_userRepository.CheckCredentials(request.Username, request.Password) 
                ? BadRequest("No user with this username and password exists.") 
                : Ok();
        }

        public ResponseContext GetCards(string token)
        {
            var user = GetUserByAuthToken(token);

            if (user is null)
            {
                return NotFound("User not found.");
            }
            
            var packageIds = _userRepository.GetPackageIds(user.Username);
            var packages = packageIds.Select(id => _packageRepository.GetPackage(id));
            var resultSet = new List<Card>();

            foreach (var package in packages)
            {
                var cardIds = package.Cards;
                cardIds.ForEach(id => resultSet.Add(_cardRepository.GetCard(id)));
            }

            return Ok(resultSet);
        }

        public ResponseContext AcquirePackage(string token)
        {
            var user = GetUserByAuthToken(token);
            
            if (user is null)
            {
                return NotFound("User not found.");
            }

            if (user.Coins == 0)
            {
                return Conflict("Not enough coins to purchase a package.");
            }

            var unownedPackage = GetUnownedPackageForUser(user.Username);

            if (unownedPackage is null)
            {
                return Conflict("You already own all the packages.");
            }

            bool purchase = _userRepository.AcquirePackage(user.Username, unownedPackage.Id);
            
            if (purchase)
            {
                _userRepository.AddCoins(user.Username, unownedPackage.Size * -1);
            }

            return Ok(unownedPackage.Id.ToString());
        }

        private User GetUserByAuthToken(string token)
        {
            var s1 = token.Split(" ");
            if (s1.Length != 2)
            {
                throw new BadRequestException();
            }

            var s2 = s1[1].Split("-");
            if (s2.Length != 2)
            {
                throw new BadRequestException("Invalid auth header.");
            }

            string username = s2[0];
            return _userRepository.GetUser(username);
        }

        private Package GetUnownedPackageForUser(string username)
        {
            var user = _userRepository.GetUser(username);

            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }
            
            var ownedPackages = _userRepository.GetPackageIds(user.Username);
            Package randomPackage; int i = 0;
            
            do
            {
                randomPackage = _packageRepository.GetRandomPackage();
                if (randomPackage is null)
                {
                    return null;
                }
                i++;
            } while (ownedPackages.Contains(randomPackage.Id) && i < 100);

            return ownedPackages.Contains(randomPackage.Id) ? null : randomPackage;
        }
    }
}