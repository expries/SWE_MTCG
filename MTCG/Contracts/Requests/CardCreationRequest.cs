using System;

namespace MTCG.Contracts.Requests
{
    public class CardCreationRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public double Weakness { get; set; }
    }
}