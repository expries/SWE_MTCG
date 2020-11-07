namespace MTCG.Resources.Cards.MonsterCards
{
    public class Kraken : MonsterCard

    {
        public Kraken(string name, double damage) : base(name, damage)
        {
            Element = Element.Water;
            MonsterType = MonsterType.Kraken;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            bool spellcard = attacker.CardType == CardType.Spellcard;
            return !spellcard && base.AttackedBy(attacker);
        }
    }
}