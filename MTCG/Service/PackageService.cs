using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.ActionResult;
using MTCG.Mapper;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Resource;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public class PackageService : IPackageService
    {
        private readonly PackageRepository _packageRepository;
        private readonly CardService _cardService;
        
        public PackageService(PackageRepository packageRepository, CardService cardService)
        {
            _packageRepository = packageRepository;
            _cardService = cardService;
        }

        public bool DeletePackage(Guid packageId)
        {
            return _packageRepository.DeletePackage(packageId);
        }

        public Package GetPackage(Guid packageId)
        {
            return _packageRepository.GetPackage(packageId);
        }

        public List<Package> GetAllPackages()
        {
            return _packageRepository.GetAllPackages();
        }

        public ActionResult<Package> CreatePackage(IEnumerable<CardCreationRequest> cardRequests)
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
                return new ActionResult<Package>(ServiceError.BadFormat, error.Message);
            }
        }

        public ActionResult<Package> CreatePackage(Package package)
        {
            if (package.Cards.Count == 0)
            {
                return new ActionResult<Package>(ServiceError.PackageIsEmpty, 
                    "A package has to contain at least one card.");
            }
            
            if (GetPackage(package.Id) != null)
            {
                return new ActionResult<Package>(ServiceError.DuplicateId, 
                    "A package with this ID already exists.");
            }
            
            var createdPackage = _packageRepository.CreatePackage(package);
            var cardCreation = new ActionResult<Card>();
            
            // create cards
            foreach (var card in createdPackage.Cards)
            {
                cardCreation = _cardService.CreateCard(card, package.Id);

                if (!cardCreation.Success)
                {
                    break;
                }
                
                createdPackage.AddCard(cardCreation.Item);
            }

            if (cardCreation.Success)
            {
                return new ActionResult<Package>(createdPackage);
            }
            
            // if not successful, delete created cards
            foreach (var card in createdPackage.Cards)
            {
                _cardService.DeleteCard(card.Id);
            }

            _packageRepository.DeletePackage(createdPackage.Id);
            return new ActionResult<Package>(ServiceError.BadFormat, cardCreation.Error.Message);
        }
    }
}