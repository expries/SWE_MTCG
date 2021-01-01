using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;

namespace MTCG.Repositories
{
    public interface ICardRepository
    {
        public List<Card> GetAll();

        public Card Get(Guid cardId);

        public Card Create(Card card, Guid packageId);
    }
}