using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public class CardRepository : ICardRepository
    {
        private readonly Dictionary<Guid, Card> _cards;

        public CardRepository()
        {
            _cards = new Dictionary<Guid, Card>();
        }

        public Card CreateCard(Card card)
        {
            if (card == null)
            {
                return null;
            }
            
            if (card.Id == Guid.Empty)
            {
                card.Id = Guid.NewGuid();
            }

            _cards.Add(card.Id, card);
            return card;
        }

        public bool DeleteCard(Guid id)
        {
            if (!_cards.ContainsKey(id))
            {
                return false;
            }

            _cards.Remove(id);
            return true;
        }

        public Card GetCard(Guid id)
        {
            return _cards.ContainsKey(id) ? _cards[id] : null;
        }

        public List<Card> GetAllCards()
        {
            return _cards.Values.ToList();
        }
    }
}