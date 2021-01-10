using MTCG.Results;

namespace MTCG.Domain.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        private Goblin(double damage) : base("Goblin", damage)
        {
            Element = Element.Normal;
            MonsterType = MonsterType.Goblin;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new Goblin(damage);
        }
    }
}