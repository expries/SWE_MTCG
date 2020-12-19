using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Mapper;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Resource;

namespace MTCG.Service
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ICardRepository _cardRepository;
        
        public PackageService(IPackageRepository packageRepository, ICardRepository cardRepository)
        {
            _packageRepository = packageRepository;
            _cardRepository = cardRepository;
        }

        public OneOf<Package, NotFound> GetPackage(Guid packageId)
        {
            var package = _packageRepository.GetPackage(packageId);

            if (package is null)
            {
                return new NotFound("No package with id " + packageId + " exists.");
            }

            return package;
        }

        public List<Package> GetAllPackages()
        {
            return _packageRepository.GetAllPackages();
        }

        public OneOf<Package, DuplicateId, Error> CreatePackage(IEnumerable<CardCreationRequest> cardRequests)
        {
            try
            {
                var cardMapper = new CardCreationRequestMapper();
                var cards = cardMapper.Map(cardRequests).ToList();
                
                var package = new Package(Guid.NewGuid());
                cards.ForEach(card => package.AddCard(card));
                return CreatePackage(package);
            }
            catch (ArgumentException error)
            {
                return new BadFormat(error.Message);
            }
        }

        public OneOf<Package, DuplicateId, Error> CreatePackage(Package package)
        {
            if (package.Cards.Count == 0)
            {
                return new PackageIsEmpty("The package may not be empty.");
            }
            
            if (GetPackage(package.Id) != null)
            {
                return new DuplicateId("A package with this ID already exists.");
            }
            
            var cardIds = package.Cards.Select(card => card.Id);
            var existentCards = _cardRepository.GetAllCards().Where(card => cardIds.Contains(card.Id)).ToList();

            if (existentCards.Any())
            {
                return new DuplicateId(
                    "Cards with these ids already exist: " + string.Join(",", existentCards));
            }
            
            var newPackage = _packageRepository.CreatePackage(package);

            foreach (var card in package.Cards)
            {
                try
                {
                    var newCard = _cardRepository.CreateCard(card, package.Id);
                    newPackage.AddCard(newCard);
                }
                catch
                {
                    _packageRepository.DeletePackage(newPackage.Id);
                    throw;  // rethrow after deleting package
                }
            }

            return newPackage;
        }
    }
}