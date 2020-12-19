namespace MTCG.Resource.Cards
{
    public abstract class MonsterCard : Card
    {
        public MonsterType MonsterType { get; protected set; }

        protected MonsterCard(string name, double damage) : base(name, damage)
        {
            CardType = CardType.Monster;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            return attacker.Damage > Damage;
        }
    }
}