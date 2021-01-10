using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Mappers;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;

namespace MTCG.Controllers
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
                return Unauthorized("Authorization is required.");
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Unauthorized("Authentication with the provided token failed.");
            }

            if (user.Username != "admin")
            {
                return Forbidden("This function is limited to admins.");
            }

            var createCards = CardCreationRequestMapper.Map(requests);
            var failedCreations = createCards.Where(x => !x.Success).ToList();

            if (failedCreations.Any())
            {
                var result = failedCreations.First();
                return BadRequest(result.Error);
            }

            var cards = createCards.Select(x => x.Value).ToList();
            var existentCards = cards
                .Select(x => x.Id)
                .Where(x => _cardRepository.Get(x) != null).ToList();

            if (existentCards.Any())
            {
                return Conflict(
                    "Cards with the following IDs already exist: " + string.Join(",", existentCards));
            }
            
            var createPackage = Package.Create(cards);

            if (createPackage.Success)
            {
                var package = createPackage.Value;
                var newPackage = _packageRepository.Create(package);
                package.Cards.ForEach(card => _cardRepository.Create(card, newPackage.Id));
                return Created(newPackage);
            }

            if (createPackage.HasError<CardAlreadyInPackage>())
            {
                return Conflict("The package contains duplicate card IDs.");
            }
            
            return BadRequest(createPackage.Error);
        }

        public ResponseContext Get(Guid packageId)
        {
            var package = _packageRepository.Get(packageId);

            if (package is null)
            {
                return NotFound("Could not find a package with id " + packageId + ".");
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