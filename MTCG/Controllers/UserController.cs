using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MTCG.Contracts.Requests;
using MTCG.Exceptions;
using MTCG.Repositories;
using MTCG.Resources;
using MTCG.Resources.Cards;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controllers
{
    public class UserController
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
                throw new ConflictException("Username is taken.");
            }

            var user = _userRepository.CreateUser(request.Username, request.Password);
            
            return new ResponseContext
            {
                Status = HttpStatus.Created,
                ContentType = MediaType.Plaintext,
                Content = user.Token.ToString()
            };
        }

        public ResponseContext Get(string username)
        {
            var user = _userRepository.GetUser(username);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                Content = JsonConvert.SerializeObject(user),
                ContentType = MediaType.Json
            };
        }

        public ResponseContext GetAll()
        {
            var users = _userRepository.GetAllUsers();

            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(users)
            };
        }
        
        public ResponseContext Login(LoginRequest request)
        {
            if (!_userRepository.CheckCredentials(request.Username, request.Password))
            {
                throw new BadRequestException("No user with this username and password exists.");
            }
            
            return new ResponseContext {Status = HttpStatus.Ok};
        }

        public ResponseContext GetCards(string token)
        {
            var user = GetUserByAuthToken(token);

            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }
            
            var packageIds = _userRepository.GetPackageIds(user.Username);
            var packages = packageIds.Select(id => _packageRepository.GetPackage(id));
            var resultSet = new List<Card>();

            foreach (var package in packages)
            {
                var cardIds = package.Cards;
                cardIds.ForEach(id => resultSet.Add(_cardRepository.GetCard(id)));
            }

            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(resultSet)
            };
        }

        public ResponseContext AcquirePackage(string token)
        {
            var user = GetUserByAuthToken(token);
            
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            if (user.Coins == 0)
            {
                throw new ConflictException("Not enough coins to purchase a package.");
            }

            var unownedPackage = GetUnownedPackageForUser(user.Username);

            if (unownedPackage is null)
            {
                throw new ConflictException("You already own all the packages.");
            }

            bool purchase = _userRepository.AcquirePackage(user.Username, unownedPackage.Id);
            if (purchase)
            {
                _userRepository.AddCoins(user.Username, unownedPackage.Size * -1);
            }

            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Plaintext,
                Content = unownedPackage.Id.ToString()
            };
        }

        private User GetUserByAuthToken(string token)
        {
            var s1 = token.Split(" ");
            if (s1.Length != 2)
            {
                throw new BadRequestException("Invalid auth header.");
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
                if (randomPackage is null) return null;
                i++;

            } while (ownedPackages.Contains(randomPackage.Id) && i < 100);

            return ownedPackages.Contains(randomPackage.Id) ? null : randomPackage;
        }
    }
}