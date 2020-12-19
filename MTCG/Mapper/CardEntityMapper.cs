using System;
using MTCG.Database.Entity;
using MTCG.Resource.Cards;
using MTCG.Resource.Cards.MonsterCards;
using MTCG.Resource.Cards.SpellCards;

namespace MTCG.Mapper
{
    public class CardEntityMapper : Mapper<CardEntity, Card>
    {
        public override Card Map(CardEntity entity)
        {
            if (entity is null)
            {
                return null;
            }
            
            var card  = entity.Type is CardType.Spell ? CreateSpellCard(entity) : CreateMonsterCard(entity);
            card.Id = entity.Id;
            return card;
        }
        
        private static Card CreateSpellCard(CardEntity entity)
        {
            return entity.Element switch
            {
                Element.Normal => new NormalSpell(entity.Name, entity.Damage, 1),
                Element.Fire   => new FireSpell(entity.Name, entity.Damage, 1),
                Element.Water  => new WaterSpell(entity.Name, entity.Damage, 1),
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