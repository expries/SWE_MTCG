using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class WaterGoblin : MonsterCard
    {
        private WaterGoblin(double damage) : base("WaterGoblin", damage)
        {
            Element = Element.Water;
            MonsterType = MonsterType.WaterGoblin;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new WaterGoblin(damage);
        }
    }
}