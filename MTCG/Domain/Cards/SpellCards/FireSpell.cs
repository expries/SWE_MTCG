using MTCG.Results;

namespace MTCG.Domain.Cards.SpellCards
{
    public class FireSpell : SpellCard
    {
        private FireSpell(double damage) : base("FireSpell", damage)
        {
            Element = Element.Fire;
        }

        public static Result<Card> Create(double damage)
        {
            var validateDamage = ValidateDamage(damage);
            
            if (!validateDamage.Success)
            {
                return validateDamage.Error;
            }

            return new FireSpell(damage);
        }
    }
}