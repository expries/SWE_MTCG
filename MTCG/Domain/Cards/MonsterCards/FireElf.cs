using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class FireElf : MonsterCard
    {
        private FireElf(double damage) : base("FireElf", damage)
        {
            Element = Element.Fire;
            MonsterType = MonsterType.FireElf;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new FireElf(damage);
        }
    }
}