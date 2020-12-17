using System;
using MTCG.Repository;
using MTCG.Server;
using Newtonsoft.Json;

namespace MTCG.Controller
{
    public class CardController : ApiController
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
            return Ok(cards);
        }

        public ResponseContext Get(Guid cardId)
        {
            var card = _cardRepository.GetCard(cardId);
            return card is null 
                ? NotFound("Card with this id does not exist.") 
                : Ok(card);
        }
    }
}