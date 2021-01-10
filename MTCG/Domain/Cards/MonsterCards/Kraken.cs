using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Kraken : MonsterCard
    {
        private Kraken(double damage) : base("Kraken", damage)
        {
            Element = Element.Water;
            MonsterType = MonsterType.Kraken;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Kraken(damage);
        }
    }
}