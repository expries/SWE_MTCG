namespace MTCG.Domain.Cards.SpellCards
{
    public class FireSpell : SpellCard
    {
        public FireSpell(string name, double damage) : base(name, damage)
        {
            Element = Element.Fire;
        }
        
        protected override bool IsEffectiveAgainst(Card other)
        {
            return other.Element == Element.Normal;
        }

        protected override bool IsIneffectiveAgainst(Card other)
        {
            return other.Element == Element.Water;
        }
    }
}