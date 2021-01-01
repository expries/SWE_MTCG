using System;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Trade
    {
        public Guid Id { get; private set; }
        
        public Card CardToTrade { get; private set; }

        public User Seller { get; private set; }

        public CardType CardType { get; private set; }

        public double MinimumDamage { get; private set; }

        private Trade(Guid id, CardType cardType, double minimumDamage, Card cardToTrade, User seller)
        {
            Id = id;
            CardType = cardType;
            MinimumDamage = minimumDamage;
            CardToTrade = cardToTrade;
            Seller = seller;
        }

        public static Result<Trade> Create(Guid id, CardType type, double minimumDamage, Card card, User seller)
        {
            if (id.Equals(Guid.Empty))
            {
                return new TradeIdIsEmpty("Trade ID may not be empty.");
            }

            if (minimumDamage < 0)
            {
                return new NegativeMinimumDamage("Minimum damage may not be negative.");
            }
			
            if (card is null)
            {
                return new CardIsNull("Card may not be null");
            }
            
            if (seller is null)
            {
                return new SellerIsNull("Seller may not be null");
            }
            
            if (!seller.Stack.Select(x => x.Id).Contains(card.Id))
            {
                return new SellerDoesNotOwnCard("This user does not own the card with ID " + card.Id + ".");
            }

            return new Trade(id, type, minimumDamage, card, seller);
        }

        public Result Commit(User buyer, Card card)
        {
            if (buyer is null)
            {
                return new BuyerIsNull("Buyer may not be null.");
            }

            if (buyer.Id.Equals(Seller.Id))
            {
                return new SelfTrading("You cannot trade with yourself.");
            }

            if (card is null)
            {
                return new CardIsNull("Card may not be null.");
            }
            
            if (!buyer.Stack.Select(x => x.Id).Contains(card.Id))
            {
                return new BuyerDoesNotOwnCard("This user does not own the card with ID " + card.Id + ".");
            }

            if (Seller.Stack.Select(x => x.Id).Contains(card.Id))
            {
                return new SellerAlreadyOwnsCard("The user already owns the card with id " + card.Id + ".");
            }

            if (card.Damage < MinimumDamage)
            {
                return new TooLowDamage(
                    "The damage of the proposed card is beneath minimum damage (" + MinimumDamage + ").");
            }

            if (card.Type != CardType)
            {
                return new IncorrectCardType("The proposed card is of wrong type.");
            }

            buyer.AddToCollection(CardToTrade);
            buyer.RemoveFromCollection(card);
            Seller.AddToCollection(card);
            Seller.RemoveFromCollection(CardToTrade);
            return Result.Ok();
        }
    }
}