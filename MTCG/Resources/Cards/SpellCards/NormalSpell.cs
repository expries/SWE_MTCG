namespace MTCG.Resources.Cards.SpellCards
{
    public class NormalSpell : SpellCard
    {
        public NormalSpell(string name, double damage) : base(name, damage)
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