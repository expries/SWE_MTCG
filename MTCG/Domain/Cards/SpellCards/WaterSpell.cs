using MTCG.Results;

namespace MTCG.Domain.Cards.SpellCards
{
    public class WaterSpell : SpellCard
    {
        private WaterSpell(double damage) : base("WaterSpell", damage)
        {
            Element = Element.Water;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new WaterSpell(damage);
        }
    }
}