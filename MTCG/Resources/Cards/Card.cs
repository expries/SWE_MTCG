using System;

namespace MTCG.Resources.Cards
{
    public abstract class Card
    {
        public Element Element { get; protected set; }
        public string Name { get; }
        public double Damage { get; set; }
        protected internal CardType CardType { get; set; }

        protected Card(string name, double damage)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name may not be null or empty!");
            }
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage may not be less than zero!");
            }
            
            Name = name;
            Damage = damage;
        }

        public virtual bool Attack(Card defender)
        {
            return defender.AttackedBy(this);
        }
        
        protected internal abstract bool AttackedBy(Card attacker);
    }
}