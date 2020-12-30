namespace MTCG.Domain.Cards.MonsterCards
{
    public class Knight : MonsterCard
    {
        public Knight(string name, double damage) : base(name, damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Knight;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            if (attacker.Type != CardType.Spell)
            {
                return base.AttackedBy(attacker);
            }
            bool waterSpell = attacker.Element == Element.Water;
            return waterSpell || base.AttackedBy(attacker); 
        }
    }
}