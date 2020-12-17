namespace MTCG.Resource.Cards.SpellCards
{
    public class WaterSpell : SpellCard
    {
        public WaterSpell(string name, double damage, double weakness) : base(name, damage, weakness)
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