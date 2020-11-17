using System;
using MTCG.Exceptions;
using MTCG.Repositories;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controllers
{
    public class CardController
    {
        private readonly ICardRepository _cardRepository;
        
        public CardController()
        {
            _cardRepository = new CardRepository();   
        }

        public CardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public ResponseContext GetAll()
        {
            var cards = _cardRepository.GetAllCards();
            
            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(cards)
            };
        }

        public ResponseContext Get(Guid cardId)
        {
            var card = _cardRepository.GetCard(cardId);

            if (card is null)
            {
                throw new NotFoundException("Card with this id does not exist.");
            }

            return new ResponseContext
            {
                Status = HttpStatus.Ok,
                ContentType = MediaType.Json,
                Content = JsonConvert.SerializeObject(card)
            };
        }
    }
}