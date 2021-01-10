using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Wizard : MonsterCard
    {
        private Wizard(double damage) : base("Wizard", damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Wizard;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Wizard(damage);
        }
    }
}