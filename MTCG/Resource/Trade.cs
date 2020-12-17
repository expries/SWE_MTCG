using System;
using System.Linq;
using MTCG.Resource.Cards;

namespace MTCG.Resource
{
    public class Trade
    {
        public Guid Id { get; private set; }
        
        public Card Card { get; private set; }
        
        public User Seller { get; private set; }

        public CardType Type { get; private set; }

        public int MinimumDamage { get; private set; }

        public Trade(Guid id, CardType type, int minimumDamage)
        {
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Trade ID may not be empty.");
            }

            if (minimumDamage < 0)
            {
                throw new ArgumentException("Minimum damage may not be negative.");
            }

            Id = id;
            Type = type;
            MinimumDamage = minimumDamage;
            Card = null;
            Seller = null;
        }

        public void SetSeller(User user)
        {
            if (user.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException("User ID may not be empty.");
            }
            
            bool userOwnsCard = user.Stack.Select(card => card.Id).Contains(Card.Id);

            if (!userOwnsCard)
            {
                throw new ArgumentException("User does not own card " + Card.Id);
            }

            Seller = user;
        }

        public void SetCard(Card card)
        {
            if (card.Id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Card ID may not be empty.");
            }

            Card = card;
        }
    }
}