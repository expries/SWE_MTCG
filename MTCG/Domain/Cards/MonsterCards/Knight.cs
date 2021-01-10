using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Knight : MonsterCard
    {
        private Knight(double damage) : base("Knight", damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Knight;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Knight(damage);
        }
    }
}