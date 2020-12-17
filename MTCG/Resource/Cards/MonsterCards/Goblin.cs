namespace MTCG.Resource.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        public Goblin(string name, double damage) : base(name, damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Goblin;
        }
    }
}