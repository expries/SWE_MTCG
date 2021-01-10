using System;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain.Cards
{
    public abstract class Card
    {
        public Guid Id { get; set; }
        
        public string Name { get; }
        
        public double Damage { get; }
        
        public Element Element { get; protected set; }

        protected internal CardType Type { get; protected set; }

        private Card(Guid id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
        
        protected Card(string name, double damage) : this(Guid.NewGuid(), name, damage) {}

        protected static Result ValidateDamage(double damage)
        {
            if (damage < 0)
            {
                return new NegativeCardDamage("Card damage may not be negative.");
            }
            
            return Result.Ok();
        }
    }
}