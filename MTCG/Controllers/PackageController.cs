using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Contracts.Requests;
using MTCG.Exceptions;
using MTCG.Repositories;
using MTCG.Resources.Cards;
using MTCG.Resources.Cards.MonsterCards;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controllers
{
    public class PackageController
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
                throw new BadRequestException("A package has to contain at least one card.");
            }

            foreach (var cardRequest in requests)
            {
                if (_cardRepository.GetCard(cardRequest.Id) != null)
                {
                    throw new ConflictException("Card with id " + cardRequest.Id + " already exists.");
                }
                var card = new Goblin(cardRequest.Name, cardRequest.Damage) { Id = cardRequest.Id };
                cards.Add(card);
            }
            
            cards.ForEach(card => _cardRepository.CreateCard(card));
            var cardIds = cards.Select(card => card.Id).ToList();
            var package = _packageRepository.CreatePackage(cardIds);

            return new ResponseContext
            {
                Status = HttpStatus.Created,
                ContentType = MediaType.Plaintext,
                Content = package.Id.ToString()
            };
        }

        public ResponseContext Get(Guid id)
        {
            var package = _packageRepository.GetPackage(id);

            if (package is null)
            {
                throw new NotFoundException("No package with this id exists.");
            }
            
            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(package)
            };
        }

        public ResponseContext GetAll()
        {
            var packages = _packageRepository.GetAllPackages();
            
            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(packages)
            };
        }
    }
}