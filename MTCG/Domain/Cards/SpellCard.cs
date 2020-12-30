namespace MTCG.Domain.Cards
{
    public abstract class SpellCard : Card
    {
        protected SpellCard(string name, double damage) : base(name, damage)
        {
            Type = CardType.Spell;
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
                return 0.5;  // 50%/200% of damage dealt/taken
            }
            if (IsEffectiveAgainst(attacker))
            {
                return 1.5;  // 200%/50% of damage dealt/taken
            }
            return 1;  // 100% of damage dealt/taken
        }
    }
}