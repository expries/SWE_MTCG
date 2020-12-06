namespace MTCG.Resources.Cards.MonsterCards
{
    public class Dragon : MonsterCard
    {
        public Dragon(string name, double damage) : base(name, damage)
        {
            Element = Element.Fire;
            MonsterType = MonsterType.Dragon;
        }
        
        protected internal override bool AttackedBy(Card attacker)
        {
            if (attacker.CardType != CardType.Monster)
            {
                return base.AttackedBy(attacker);
            }
            var card = (MonsterCard) attacker;
            bool isGoblin = card.MonsterType == MonsterType.Goblin;
            return !isGoblin && base.AttackedBy(attacker);
        }
    }
}