namespace MTCG.Domain.Cards
{
    public abstract class MonsterCard : Card
    {
        public MonsterType MonsterType { get; protected set; }

        protected MonsterCard(string name, double damage) : base(name, damage)
        {
            Type = CardType.Monster;
        }
    }
}