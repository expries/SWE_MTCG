using System;
using System.Collections.Generic;
using MTCG.Resources.Cards;

namespace MTCG.Repositories
{
    public interface ICardRepository
    {
        public List<Card> GetAllCards();

        public Card GetCard(Guid id);
        
        public Card CreateCard(Card card);
    }
}