using System;
using MTCG.Repository;
using MTCG.Server;
using MTCG.Service;
using Newtonsoft.Json;

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
            return result.Match(Ok, NotFound);
        }
    }
}