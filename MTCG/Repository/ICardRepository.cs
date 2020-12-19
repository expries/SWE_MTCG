using System;
using System.Collections.Generic;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public interface ICardRepository
    {
        public List<Card> GetAllCards();

        public Card GetCard(Guid id);
        
        public Card CreateCard(Card card, Guid packageId);
    }
}