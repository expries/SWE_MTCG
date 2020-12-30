using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;

namespace MTCG.Repositories
{
    public interface ICardRepository
    {
        public List<Card> GetAllCards();

        public Card GetCard(Guid cardId);

        public Card CreateCard(Card card, Guid packageId);
    }
}