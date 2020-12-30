namespace MTCG.Domain.Cards.MonsterCards
{
    public class WaterGoblin : MonsterCard
    {
        public WaterGoblin(string name, double damage) : base(name, damage)
        {
            Element = Element.Water;
            MonsterType = MonsterType.WaterGoblin;
        }   
    }
}