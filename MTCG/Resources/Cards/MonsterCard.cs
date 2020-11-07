using System;
using System.Reflection.Metadata.Ecma335;

namespace MTCG.Resources.Cards
{
    public abstract partial class MonsterCard : Card
    {
        public MonsterType MonsterType { get; protected set; }

        protected MonsterCard(string name, double damage) : base(name, damage)
        {
            CardType = CardType.Monstercard;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            return attacker.Damage > Damage;
        }
    }
}