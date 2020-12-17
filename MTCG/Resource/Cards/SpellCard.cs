using MTCG.Exception;

namespace MTCG.Resource.Cards
{
    public abstract class SpellCard : Card
    {
        public double Weakness { get; }

        protected SpellCard(string name, double damage, double weakness) : base(name, damage)
        {
            if (weakness < 0)
            {
                throw new BadRequestException("Weakness has to be greater or equal to zero.");
            }
            
            CardType = CardType.Spell;
            Weakness = weakness;
        }
        
        public override bool Attack(Card defender)
        {
            double baseDamage = Damage;
            Damage *= ElementalBonusFactorAgainst(defender);
            bool result = defender.AttackedBy(this);
            Damage = baseDamage;
            return result;
        }
        
        protected internal override bool AttackedBy(Card attacker)
        {
            double attackingDamage = attacker.Damage / ElementalBonusFactorAgainst(attacker);
            return attackingDamage > Damage;
        }

        protected virtual bool IsEffectiveAgainst(Card other)
        {
            return false;
        }
        
        protected virtual bool IsIneffectiveAgainst(Card other)
        {
            return false;
        }

        private double ElementalBonusFactorAgainst(Card attacker)
        {
            if (IsIneffectiveAgainst(attacker))
            {
                return Weakness / 100;  // 50%/200% of damage dealt/taken
            }
            if (IsEffectiveAgainst(attacker))
            {
                return 1 + Weakness / 100;  // 200%/50% of damage dealt/taken
            }
            return 1;  // 100% of damage dealt/taken
        }
    }
}