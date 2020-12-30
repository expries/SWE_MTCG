using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using MTCG.Entities;

namespace MTCG.Mappers
{
    public static class CardEntityMapper
    {
        public static List<Card> Map(IEnumerable<CardEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        
        public static Card Map(CardEntity entity)
        {
            if (entity is null)
            {
                return null;
            }

            var card = CreateCard(entity);
            card.Id = entity.Id;
            return card;
        }

        private static Card CreateCard(CardEntity entity)
        {
            return entity.Type is CardType.Spell 
                ? CreateSpellCard(entity) 
                : CreateMonsterCard(entity);
        }
        
        private static Card CreateSpellCard(CardEntity entity)
        {
            return entity.Element switch
            {
                Element.Normal => new NormalSpell(entity.Name, entity.Damage),
                Element.Fire   => new FireSpell(entity.Name, entity.Damage),
                Element.Water  => new WaterSpell(entity.Name, entity.Damage),
                _ => throw new ArgumentException("Card has an unknown Element: " + entity.Element)
            };
        }

        private static Card CreateMonsterCard(CardEntity entity)
        {
            return entity.MonsterType switch
            {
                MonsterType.WaterGoblin => new WaterGoblin(entity.Name, entity.Damage),
                MonsterType.Goblin      => new Goblin(entity.Name, entity.Damage),
                MonsterType.Dragon      => new Dragon(entity.Name, entity.Damage),
                MonsterType.Wizard      => new Wizard(entity.Name, entity.Damage),
                MonsterType.Ork         => new Ork(entity.Name, entity.Damage),
                MonsterType.Knight      => new Knight(entity.Name, entity.Damage),
                MonsterType.Kraken      => new Kraken(entity.Name, entity.Damage),
                MonsterType.FireElf     => new FireElf(entity.Name, entity.Damage),
                _ => throw new ArgumentException("Card is an unknown monster type: " + entity.MonsterType)
            };
        }
    }
}