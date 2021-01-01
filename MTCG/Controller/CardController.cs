using System;
using MTCG.Repositories;
using MTCG.Server;

namespace MTCG.Controller
{
    public class CardController : ApiController
    {
        private readonly ICardRepository _cardRepository;

        public CardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public ResponseContext GetAll()
        {
            var cards = _cardRepository.GetAll();
            return Ok(cards);
        }

        public ResponseContext Get(Guid cardId)
        {
            var card = _cardRepository.Get(cardId);

            if (card is null)
            {
                return NotFound(new {Error = "Found no card with id " + cardId + "."});
            }
            
            return Ok(card);
        }
    }
}