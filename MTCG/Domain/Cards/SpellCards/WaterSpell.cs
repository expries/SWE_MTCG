namespace MTCG.Domain.Cards.SpellCards
{
    public class WaterSpell : SpellCard
    {
        public WaterSpell(string name, double damage) : base(name, damage)
        {
            Element = Element.Water;
        }
        
        protected override bool IsEffectiveAgainst(Card other)
        {
            return other.Element == Element.Fire;
        }

        protected override bool IsIneffectiveAgainst(Card other)
        {
            return other.Element == Element.Normal;
        }
    }
}