using System;
using System.Collections.Generic;
using MTCG.Resource.Cards;

namespace MTCG.Resource
{
    public class Package
    {
        public Guid Id { get; }
        
        public int Price => Cards.Count;
        
        public List<Card> Cards { get; private set; }

        public Package()
        {
            Cards = new List<Card>();
        }

        public Package(Guid id) : this()
        {
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Package ID may not be empty.");
            }
            
            Id = id;
        }

        public void AddCard(Card card)
        {
            if (card.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Card ID may not be empty.");
            }

            Cards.Add(card);
        }

        public void Clear()
        {
            Cards.Clear();
        }
    }
}