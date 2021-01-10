using System.Collections.Generic;
using System.Linq;
using MTCG.Domain.Cards;
using MTCG.Domain.Cards.MonsterCards;
using MTCG.Domain.Cards.SpellCards;
using MTCG.Entities;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Mappers
{
    public static class CardEntityMapper
    {
        public static List<Card> MapIgnoreErrors(List<CardEntity> entities)
        {
            return entities.Select(MapIgnoreError).ToList();
        }
        
        public static List<Result<Card>> Map(List<CardEntity> entities)
        {
            return entities.Select(Map).ToList();
        }

        public static Card MapIgnoreError(CardEntity entity)
        {
            var mapping = Map(entity);
            return mapping?.Value;
        }
        
        public static Result<Card> Map(CardEntity entity)
        {
            if (entity is null)
            {
                return null;
            }

            var createCard = CreateCard(entity);

            if (!createCard.Success)
            {
                return createCard.Error;
            }
            
            var card = createCard.Value;
            card.Id = entity.Id;
            return card;
        }

        private static Result<Card> CreateCard(CardEntity entity)
        {
            return entity.Type is CardType.Spell 
                ? CreateSpellCard(entity) 
                : CreateMonsterCard(entity);
        }
        
        private static Result<Card> CreateSpellCard(CardEntity entity)
        {
            return entity.Element switch
            {
                Element.Normal => RegularSpell.Create(entity.Damage),
                Element.Fire   => FireSpell.Create(entity.Damage),
                Element.Water  => WaterSpell.Create(entity.Damage),
                _ => new UnknownElement("Card has an unknown Element: " + entity.Element)
            };
        }

        private static Result<Card> CreateMonsterCard(CardEntity entity)
        {
            return entity.MonsterType switch
            {
                MonsterType.WaterGoblin => WaterGoblin.Create(entity.Damage),
                MonsterType.Goblin      => Goblin.Create(entity.Damage),
                MonsterType.Dragon      => Dragon.Create(entity.Damage),
                MonsterType.Wizard      => Wizard.Create(entity.Damage),
                MonsterType.Ork         => Ork.Create(entity.Damage),
                MonsterType.Knight      => Knight.Create(entity.Damage),
                MonsterType.Kraken      => Kraken.Create(entity.Damage),
                MonsterType.FireElf     => FireElf.Create(entity.Damage),
                _ => new UnknownMonsterType("Card is an unknown monster type: " + entity.MonsterType)
            };
        }
    }
}