using System;
using MTCG.Server;
using MTCG.Services;

namespace MTCG.Controller
{
    public class CardController : ApiController
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        public ResponseContext GetAll()
        {
            var cards = _cardService.GetAllCards();
            return Ok(cards);
        }

        public ResponseContext Get(Guid cardId)
        {
            var result = _cardService.GetCard(cardId);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }
            
            var card = result.Value;
            return Ok(card);
        }
    }
}