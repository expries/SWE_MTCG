using System;
using MTCG.Database;
using MTCG.Domain.Cards;

namespace MTCG.Entities
{
    public class TradeEntity
    {
        [Column(Name="tradeID")]
        public Guid Id { get; set; }
        
        [Column(Name="fk_cardID")]
        public Guid CardId { get; set; }
        
        [Column(Name="fk_userID")]
        public Guid UserId { get; set; }
        
        [Column(Name="cardType")]
        public CardType Type { get; set; }
        
        [Column(Name="minimumDamage")]
        public int MinimumDamage { get; set; }
    }
}