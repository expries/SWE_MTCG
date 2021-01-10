using MTCG.Domain.Cards.SpellCards;
using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Dragon : MonsterCard
    {
        private Dragon(double damage) : base("Dragon", damage)
        {
            Element = Element.Fire;
            MonsterType = MonsterType.Dragon;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Dragon(damage);
        }
    }
}