using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Database.Entity;
using MTCG.Mapper;
using MTCG.Resource.Cards;

namespace MTCG.Repository
{
    public class CardRepository : ICardRepository
    {
        private readonly DatabaseManager _db;

        public CardRepository(DatabaseManager db)
        {
            _db = db;
        }

        public Card CreateCard(Card card, Guid packageId)
        {
            const string sql = "INSERT INTO card (cardid, name, damage, element, type, fk_packageid) " +
                               "VALUES (@cardID, @name, @damage, @element, @type, @fk_packageId)";

            var cardData =
                new {cardID = card.Id, name = card.Name, damage = card.Damage, element = card.Element,
                    type = card.CardType, fk_packageId = packageId};
            
            _db.ExecuteNonQuery(sql, cardData);

            if (card.CardType is CardType.Monster)
            {
                CreateMonsterCard(card);
            }
            
            return GetCard(card.Id);
        }

        private void CreateMonsterCard(Card card)
        {
            const string sql = "INSERT INTO monster_card (fk_cardid, monstertype) " +
                               "VALUES (@cardID, @monsterType";

            var monster = (MonsterCard) card;
            _db.ExecuteNonQuery(sql, new {cardId = card.Id, monsterType = monster.MonsterType});
        }

        public Card GetCard(Guid id)
        {
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype FROM card_info " + 
                               "WHERE cardid = @cardId";

            var entity = _db.FetchFromQuery<CardEntity>(sql, new {cardId = id}).FirstOrDefault();

            if (entity is null)
            {
                return null;
            }
            
            var cardMapper = new CardEntityMapper();
            var card = cardMapper.Map(entity);
            return card;
        }

        public List<Card> GetAllCards()
        {
            const string sql = "SELECT cardid, name, type, element, damage, fk_packageid, monstertype FROM card_info";
            var entities = _db.FetchFromQuery<CardEntity>(sql);
            var cardMapper = new CardEntityMapper();
            var cards = cardMapper.Map(entities).ToList();
            return cards;
        }
    }
}