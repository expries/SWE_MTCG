using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;
using MTCG.Repositories;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public class CardService : ICardService
    {
        private readonly CardRepository _cardRepository;

        public CardService(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Result<Card> GetCard(Guid cardId)
        {
            var card = _cardRepository.GetCard(cardId);

            if (card is null)
            {
                return new CardNotFound("No card with id " + cardId + " exists.");
            }

            return card;
        }

        public List<Card> GetAllCards()
        {
            return _cardRepository.GetAllCards();
        }
    }
}