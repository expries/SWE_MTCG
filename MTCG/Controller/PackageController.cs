using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Repository;
using MTCG.Request;
using MTCG.Resource.Cards;
using MTCG.Resource.Cards.MonsterCards;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controller
{
    public class PackageController : ApiController
    {
        private readonly ICardRepository _cardRepository;
        private readonly IPackageRepository _packageRepository;
        
        public PackageController()
        {
            _cardRepository = new CardRepository();
            _packageRepository = new PackageRepository();
        }

        public PackageController(ICardRepository cardRepository, IPackageRepository packageRepository)
        {
            _cardRepository = cardRepository;
            _packageRepository = packageRepository;
        }

        public ResponseContext Create(IEnumerable<CardCreationRequest> requests)
        {
            var cards = new List<Card>();
            requests = requests.ToList();
            
            if (!requests.Any())
            {
                return BadRequest("A package has to contain at least one card.");
            }

            foreach (var cardRequest in requests)
            {
                if (_cardRepository.GetCard(cardRequest.Id) != null)
                {
                    return Conflict("Card with id " + cardRequest.Id + " already exists.");
                }
                
                var card = new Goblin(cardRequest.Name, cardRequest.Damage) { Id = cardRequest.Id };
                cards.Add(card);
            }
            
            cards.ForEach(card => _cardRepository.CreateCard(card));
            var cardIds = cards.Select(card => card.Id).ToList();
            var package = _packageRepository.CreatePackage(cardIds);

            return Created(package.Id.ToString());
        }

        public ResponseContext Get(Guid id)
        {
            var package = _packageRepository.GetPackage(id);

            return package is null 
                ? NotFound("No package with this id exists.") 
                : Ok(package);
        }

        public ResponseContext GetAll()
        {
            var packages = _packageRepository.GetAllPackages();
            return Ok(packages);
        }
    }
}