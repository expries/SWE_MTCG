using System;

namespace MTCG.Requests
{
    public class CardCreationRequest
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public double Damage { get; set; }
    }
}