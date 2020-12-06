namespace MTCG.Resources.Cards.MonsterCards
{
    public class FireElf : MonsterCard
    {
        public FireElf(string name, double damage) : base(name, damage)
        {
            Element = Element.Fire;
            MonsterType = MonsterType.FireElf;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            if (attacker.CardType != CardType.Monster)
            {
                return base.AttackedBy(attacker);
            }
            var card = (MonsterCard) attacker;
            bool isDragon = card.MonsterType == MonsterType.Dragon;
            return !isDragon && base.AttackedBy(attacker);
        }
    }
}