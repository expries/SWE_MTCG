using System.Collections.Generic;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Mappers
{
    public static class CardCreationRequestMapper
    {
        public static List<Result<Card>> Map(List<CardCreationRequest> requests)
        {
            return requests.Select(Map).ToList();
        }
        
        public static Result<Card> Map(CardCreationRequest request)
        {
            if (request is null)
            {
                return null;
            }
            
            var createCard = CreateCard(request);

            if (!createCard.Success)
            {
                return createCard.Error;
            }

            var card = createCard.Value;
            card.Id = request.Id;
            return card;
        }

        private static Result<Card> CreateCard(CardCreationRequest obj)
        {
            return obj.Name switch
            {
                "RegularSpell" => RegularSpell.Create(obj.Damage),
                "FireSpell"    => FireSpell.Create(obj.Damage),
                "WaterSpell"   => WaterSpell.Create(obj.Damage),
                "WaterGoblin"  => WaterGoblin.Create(obj.Damage),
                "Goblin"       => Goblin.Create(obj.Damage),
                "Dragon"       => Dragon.Create(obj.Damage),
                "Wizard"       => Wizard.Create(obj.Damage),
                "Ork"          => Ork.Create(obj.Damage),
                "Knight"       => Knight.Create(obj.Damage),
                "Kraken"       => Kraken.Create(obj.Damage),
                "FireElf"      => FireElf.Create(obj.Damage),
                _ => new UnknownCardName("Card is of an unknown type: " + obj.Name)
            };
        }
    }
}