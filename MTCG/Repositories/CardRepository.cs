using System;
using System.Collections.Generic;
using System.Linq;
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
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype " + 
                               "FROM card_info WHERE cardid = @cardId";

            var entity = _db.QueryFirstOrDefault<CardEntity>(sql, new {cardId = cardId});
            
            if (entity is null)
            {
                return null;
            }
            
            var card = CardEntityMapper.Map(entity);
            return card;
        }

        public List<Card> GetAll()
        {
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype " + 
                               "FROM card_info";
            
            var entities = _db.Query<CardEntity>(sql);
            var cards = CardEntityMapper.Map(entities).ToList();
            return cards;
        }
        
        public Card Create(Card card, Guid packageId)
        {
            const string sql = "INSERT INTO card (cardid, name, damage, element, type, fk_packageid) " +
                               "VALUES (@cardID, @name, @damage, @element, @type, @fk_packageId)";

            var cardData =
                new {cardID = card.Id, name = card.Name, damage = card.Damage, element = card.Element,
                    type = card.Type, fk_packageId = packageId};
            
            _db.ExecuteNonQuery(sql, cardData);

            if (card.Type.Equals(CardType.Monster))
            {
                CreateMonsterCard(card);
            }

            return card;
        }

        private void CreateMonsterCard(Card card)
        {
            const string sql = "INSERT INTO monster_card (fk_cardid, monstertype) " +
                               "VALUES (@cardID, @monsterType)";

            var monster = (MonsterCard) card;
            _db.ExecuteNonQuery(sql, new {cardId = card.Id, monsterType = monster.MonsterType});
        }
    }
}