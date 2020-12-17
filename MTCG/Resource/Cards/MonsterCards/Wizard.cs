namespace MTCG.Resource.Cards.MonsterCards
{
    public class Wizard : MonsterCard
    {
        public Wizard(string name, double damage) : base(name, damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Wizard;
        }

        protected internal override bool AttackedBy(Card attacker)
        {
            if (attacker.CardType != CardType.Monster)
            {
                return base.AttackedBy(attacker);
            }
            var card = (MonsterCard) attacker;
            bool ork = card.MonsterType == MonsterType.Ork;
            return !ork && base.AttackedBy(attacker);
        }
    }
}