using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Ork : MonsterCard
    {
        private Ork(double damage) : base("Ork", damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Ork;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Ork(damage);
        }
    }
}