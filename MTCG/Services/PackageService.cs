using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MTCG.Domain;
using MTCG.Mappers;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;

        public PackageService(IPackageRepository packageRepository, ICardRepository cardRepository, IUserRepository userRepository)
        {
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
            _userRepository = userRepository;
        }

        public Result<Package> GetPackage(Guid packageId)
        {
            var package = _packageRepository.GetPackage(packageId);

            if (package is null)
            {
                return new PackageNotFound("No package with id " + packageId + " exists.");
            }

            return package;
        }

        public List<Package> GetAllPackages()
        {
            return _packageRepository.GetAllPackages();
        }

        public Result<Package> CreatePackage(Package package, string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null || user.Username != "admin")
            {
                return new NotPermitted("This function is limited to admins.");
            }

            if (package.Cards.Count != 5)
            {
                return new PackageDoesNotConsistOf5("A package has to consist of exactly 5 cards.");
            }
            
            if (_packageRepository.GetPackage(package.Id) != null)
            {
                return new DuplicatePackageId("A package with this ID already exists.");
            }
            
            var existentCards = package.Cards.Where(card => _cardRepository.GetCard(card.Id) != null).ToList();

            if (existentCards.Any())
            {
                return new DuplicateCardId(
                    "Cards with these IDs already exist: " + string.Join(",", existentCards));
            }
            
            _packageRepository.CreatePackage(package);
            package.Cards.ForEach(card => _cardRepository.CreateCard(card, package.Id));
            return package;
        }
    }
}