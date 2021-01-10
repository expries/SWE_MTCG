using System;
using System.Collections.Generic;
using System.Reflection;
using MTCG.Database;
using MTCG.Domain.Cards;
using MTCG.Entities;
using MTCG.Mappers;

namespace MTCG.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly DatabaseManager _db;

        public CardRepository(DatabaseManager db)
        {
            _db = db;
        }

        public Card Get(Guid cardId)
        {
            const string sql = "SELECT cardID, name, type, element, damage, fk_packageID, monsterType " + 
                               "FROM card WHERE cardID = @CardId";

            var entity = _db.QueryFirstOrDefault<CardEntity>(sql, new {CardId = cardId});
            var card = CardEntityMapper.MapIgnoreError(entity);
            return card;
        }

        public List<Card> GetAll()
        {
            const string sql = "SELECT cardID, name, type, element, damage, fk_packageID, monsterType " + 
                               "FROM card";
            
            var entities = _db.Query<CardEntity>(sql);
            var cards = CardEntityMapper.MapIgnoreErrors(entities);
            return cards;
        }
        
        public Card Create(Card card, Guid packageId)
        {
            const string sql = "INSERT INTO card (cardid, name, damage, element, type, monsterType, fk_packageid) " +
                               "VALUES (@CardId, @Name, @Damage, @Element, @Type, @MonsterType, @Fk_packageId)";

            var properties= card is MonsterCard monsterCard
                ? GetMonsterCardProperties(monsterCard, packageId)
                : GetCardProperties(card, packageId);

            _db.Execute(sql, properties);
            return card;
        }

        private static object GetCardProperties(Card card, Guid packageId)
        {
            return new {
                CardId = card.Id, 
                Name = card.Name, 
                Damage = card.Damage, 
                Element = card.Element,
                Type = card.Type, 
                MonsterType = DBNull.Value,
                Fk_packageId = packageId
            };
        }
        
        private static object GetMonsterCardProperties(MonsterCard card, Guid packageId)
        {
            return new {
                CardId = card.Id, 
                Name = card.Name, 
                Damage = card.Damage, 
                Element = card.Element,
                Type = card.Type, 
                MonsterType = card.MonsterType,
                Fk_packageId = packageId
            };
        }
    }
}