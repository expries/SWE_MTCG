using System;
using MTCG.Domain.Cards;

namespace MTCG.Requests
{
    public class TradeCreationRequest
    {
        public Guid Id { get; set; }

        public Guid CardToTrade { get; set; }
        
        public CardType Type { get; set; }

        public double MinimumDamage { get; set; }
    }
}