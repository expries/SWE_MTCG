using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using MTCG.Requests;

namespace MTCG.Mappers
{
    public static class CardCreationRequestMapper
    {
        public static List<Card> Map(IEnumerable<CardCreationRequest> entities)
        {
            return entities.Select(Map).ToList();
        }
        
        public static Card Map(CardCreationRequest obj)
        {
            if (obj is null)
            {
                return null;
            }
            
            var card = CreateCard(obj);
            card.Id = obj.Id;
            return card;
        }

        private static Card CreateCard(CardCreationRequest obj)
        {
            return obj.Name switch
            {
                "NormalSpell" => new NormalSpell(obj.Name, obj.Damage),
                "FireSpell"   => new FireSpell(obj.Name, obj.Damage),
                "WaterSpell"  => new WaterSpell(obj.Name, obj.Damage),
                "WaterGoblin" => new WaterGoblin(obj.Name, obj.Damage),
                "Goblin"      => new Goblin(obj.Name, obj.Damage),
                "Dragon"      => new Dragon(obj.Name, obj.Damage),
                "Wizard"      => new Wizard(obj.Name, obj.Damage),
                "Ork"         => new Ork(obj.Name, obj.Damage),
                "Knight"      => new Knight(obj.Name, obj.Damage),
                "Kraken"      => new Kraken(obj.Name, obj.Damage),
                "FireElf"     => new FireElf(obj.Name, obj.Damage),
                _ => throw new ArgumentException("Card is of an unknown type: " + obj.Name)
            };
        }
    }
}