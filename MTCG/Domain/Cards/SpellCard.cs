namespace MTCG.Domain.Cards
{
    public abstract class SpellCard : Card
    {
        protected SpellCard(string name, double damage) : base(name, damage)
        {
            Type = CardType.Spell;
        }
    }
}