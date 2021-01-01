using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Domain
{
    public class Package
    {
        public Guid Id { get; }
        
        public int Price => Cards.Count;
        
        public List<Card> Cards { get; }

        private Package()
        {
            Cards = new List<Card>();
            Id = Guid.NewGuid();
        }
        
        private Package(Guid id)
        {
            Cards = new List<Card>();
            Id = id;
        }
        
        public static Package Create()
        {
            return new Package();
        }

        public static Result<Package> Create(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new PackageIdIsEmpty("Package id may not be empty.");
            }

            return new Package(id);
        }

        public static Result<Package> Create(List<Card> cards)
        {
            var id = Guid.NewGuid();
            return Create(id, cards);
        }

        public static Result<Package> Create(Guid id, List<Card> cards)
        {
            var creation = Create(id);

            if (!creation.Success)
            {
                return creation.Error;
            }

            var package = creation.Value;

            foreach (var card in cards)
            {
                var addCard = package.AddCard(card);
                
                if (!addCard.Success)
                {
                    return addCard.Error;
                }
            }

            return package;
        }

        public Result AddCard(Card card)
        {
            if (Cards.Select(x => x.Id).Contains(card.Id))
            {
                return new CardAlreadyInPackage("Package already contains a card with ID " + card.Id + ".");
            }

            Cards.Add(card);
            return Result.Ok();
        }
    }
}