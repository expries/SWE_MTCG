using System;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Trade
    {
        public Guid Id { get; }
        
        public Card CardToTrade { get; }

        public User Seller { get; }

        public CardType CardType { get; }

        public double MinimumDamage { get; }

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
            
            if (seller.Stack.All(x => x.Id != card.Id))
            {
                return new SellerDoesNotOwnCard("User does not own the card with ID " + card.Id + ".");
            }

            if (seller.Deck.Any(x => x.Id == card.Id))
            {
                return new CardAlreadyInDeck("Card can't be traded: Card is in user deck");
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
                return new SelfTrading("Users may not trade with themselves.");
            }

            if (card is null)
            {
                return new CardIsNull("Card may not be null.");
            }
            
            if (buyer.Stack.All(x => x.Id != card.Id))
            {
                return new BuyerDoesNotOwnCard("This user does not own the card with ID " + card.Id + ".");
            }
            
            if (buyer.Deck.Any(x => x.Id == card.Id))
            {
                return new CardAlreadyInDeck("Card can't be traded: Card is in user deck");
            }

            if (Seller.Stack.Any(x => x.Id == card.Id))
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

            buyer.AddToStack(CardToTrade);
            buyer.RemoveFromStack(card);
            Seller.AddToStack(card);
            Seller.RemoveFromStack(CardToTrade);
            
            return Result.Ok();
        }
    }
}