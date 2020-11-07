namespace MTCG.Resources.Cards.MonsterCards
{
    public class Ork : MonsterCard
    {
        public Ork(string name, double damage) : base(name, damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Ork;
        }
    }
}