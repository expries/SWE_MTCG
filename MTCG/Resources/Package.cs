using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MTCG.Exceptions;
using Newtonsoft.Json;

namespace MTCG.Resources
{
    public class Package
    {
        public Guid Id { get; set; }
        
        public int Size => Cards.Count;

        [JsonProperty("Cards")]
        public List<Guid> Cards { get; private set; }

        public Package()
        {
            Cards = new List<Guid>();
        }

        public Package(Guid id) : this()
        {
            Id = id;
        }

        public void SetCards(List<Guid> cardIds)
        {
            for (int i = 0; i < cardIds.Count; i++)
            {
                if (cardIds[i] == Guid.Empty)
                {
                    throw new BadRequestException("Card at position " + i + " is missing an id.");
                }
            }
            
            Cards = cardIds;
        }
    }
}