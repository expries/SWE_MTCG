using MTCG.Results;

namespace MTCG.Domain.Cards.SpellCards
{
    public class RegularSpell : SpellCard
    {
        private RegularSpell(double damage) : base("RegularSpell", damage)
        {
            Element = Element.Normal;
        }
        
        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new RegularSpell(damage);
        }
    }
}