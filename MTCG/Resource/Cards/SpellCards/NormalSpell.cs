namespace MTCG.Resource.Cards.SpellCards
{
    public class NormalSpell : SpellCard
    {
        public NormalSpell(string name, double damage, double weakness) : base(name, damage, weakness)
        {
            Element = Element.Normal;
        }

        protected override bool IsEffectiveAgainst(Card other)
        {
            return other.Element == Element.Water;
        }

        protected override bool IsIneffectiveAgainst(Card other)
        {
            return other.Element == Element.Fire;
        }
    }
}