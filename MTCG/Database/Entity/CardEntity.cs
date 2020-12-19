using System;
using MTCG.Resource.Cards;

namespace MTCG.Database.Entity
{
    public class CardEntity
    {
        [Column(Name="cardID")]
        public Guid Id { get; set; }
        
        [Column(Name="name")]
        public string Name { get; set; }
        
        [Column(Name="damage")]
        public double Damage { get; set; }
        
        [Column(Name="element")]
        public Element Element { get; set; }

        [Column(Name="fk_packageID")] 
        public Guid PackageId { get; set; }

        [Column(Name="type")] 
        public CardType Type { get; set; }
        
        [Column(Name="monsterType")] 
        public MonsterType MonsterType { get; set; }
    }
}