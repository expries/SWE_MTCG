using System;
using System.Collections.Generic;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Repository;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public class CardService : ICardService
    {
        private readonly CardRepository _cardRepository;

        public CardService(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public OneOf<Card, NotFound> GetCard(Guid cardId)
        {
            var card = _cardRepository.GetCard(cardId);

            if (card is null)
            {
                return new NotFound("No card with id " + cardId + " exists.");
            }

            return card;
        }

        public List<Card> GetAllCards()
        {
            return _cardRepository.GetAllCards();
        }
    }
}