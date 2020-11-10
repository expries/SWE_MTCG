using System;
using System.Collections.Generic;
using MTCG.Resources.Cards;
using MTCG.Resources.Cards.MonsterCards;
using MTCG.Resources.Cards.SpellCards;

namespace MTCG.Repositories
{
    public class CardRepository
    {
        private Dictionary<Guid, Card> _cards;

        public CardRepository()
        {
            _cards = new Dictionary<Guid, Card>();
        }

        public Card CreateCard(string name, double damage)
        {
            var id = Guid.NewGuid();
            return CreateCard(id, name, damage);
        }

        public Card CreateCard(string name, double damage, double weakness)
        {
            var id = Guid.NewGuid();
            return CreateCard(id, name, damage, weakness);
        }

        public Card CreateCard(Guid id, string name, double damage)
        {
            if (_cards.ContainsKey(id))
            {
                return null;
            }
            
            Card card = new Goblin(name, damage);
            card.Id = id;
            _cards.Add(id, card);
            return card;
        }

        public Card CreateCard(Guid id, string name, double damage, double weakness)
        {
            if (_cards.ContainsKey(id))
            {
                return null;
            }

            Card card = name switch
            {
                "WaterSpell" => new WaterSpell(name, damage),
                "FireSpell" => new FireSpell(name, damage),
                "RegularSpell" => new NormalSpell(name, damage),
                _ => null
            };

            if (card == null) return null;
            
            card.Id = id;
            _cards.Add(id, card);
            return card;
        }

        public bool DeleteCard(Guid id)
        {
            if (!_cards.ContainsKey(id)) return false;
            _cards.Remove(id);
            return true;
        }

        public Card GetCard(Guid id)
        {
            return _cards.ContainsKey(id) ? _cards[id] : null;
        }
    }
}