using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Mappers;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;

namespace MTCG.Controller
{
    public class PackageController : ApiController
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;

        public PackageController(IPackageRepository packageRepository, ICardRepository cardRepository, IUserRepository userRepository)
        {
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
            _userRepository = userRepository;
        }

        public ResponseContext Create(string token, List<CardCreationRequest> requests)
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

            if (user.Username != "admin")
            {
                return Forbidden(new {Error = "This function is limited to admins."});
            }
            
            var cards = CardCreationRequestMapper.Map(requests).ToList();
            var existentCards = cards.Where(x => _cardRepository.Get(x.Id) != null).ToList();

            if (existentCards.Any())
            {
                return Conflict(new
                {
                    Error = "Cards with the following IDs already exist: " + string.Join(",", existentCards)
                });
            }
            
            var createPackage = Package.Create(cards);

            if (createPackage.Success)
            {
                var package = createPackage.Value;
                var newPackage = _packageRepository.Create(package);
                package.Cards.ForEach(card => _cardRepository.Create(card, newPackage.Id));
                return Ok(newPackage);
            }

            if (createPackage.HasError<CardAlreadyInPackage>())
            {
                return Conflict(new {Error = "The package contains duplicate card IDs."});
            }
            
            return BadRequest(createPackage.Error);
        }

        public ResponseContext Get(Guid packageId)
        {
            var package = _packageRepository.Get(packageId);

            if (package is null)
            {
                return NotFound(new {Error = "Could not find a package with id " + packageId + "."});
            }
            
            return Ok(package);
        }

        public ResponseContext GetAll()
        {
            var packages = _packageRepository.GetAll();
            return Ok(packages);
        }
    }
}