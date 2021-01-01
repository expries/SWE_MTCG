using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Database;
using MTCG.Domain;
using MTCG.Entities;

namespace MTCG.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly DatabaseManager _db ;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;

        public TradeRepository(DatabaseManager db, IUserRepository userRepository, ICardRepository cardRepository)
        {
            _db = db;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
        }

        public List<Trade> GetAll()
        {
            const string sql = "SELECT tradeID, fk_cardID, fk_userID, cardType, minimumDamage " +
                               "FROM trade";

            var entities = _db.Query<TradeEntity>(sql);
            var trades = entities.Select(GetTradeFromEntity).ToList();
            return trades;
        }

        public Trade Get(Guid tradeId)
        {
            const string sql = "SELECT tradeID, fk_cardID, fk_userID, cardType, minimumDamage " +
                               "FROM trade WHERE tradeId = @TradeID";

            var entity = _db.QueryFirstOrDefault<TradeEntity>(sql, new {TradeID = tradeId});
            var trade = GetTradeFromEntity(entity);
            return trade;
        }

        public Trade Create(Trade trade)
        {
            const string sql = "INSERT INTO trade (tradeID, fk_cardID, fk_userID, cardType, minimumDamage) " +
                               "VALUES (@TradeId, @CardId, @UserId, @CardType, @MinimumDamage)";

            _db.ExecuteNonQuery(sql, new
            {
                TradeId = trade.Id,
                CardId = trade.CardToTrade.Id,
                UserId = trade.Seller.Id,
                CardType = trade.CardType,
                MinimumDamage = trade.MinimumDamage
            });

            return Get(trade.Id);
        }

        public void Delete(Trade trade)
        {
            const string sql = "DELETE FROM trade WHERE tradeID = @TradeId";
            _db.ExecuteNonQuery(sql, new {TradeId = trade.Id});
        }
        
        private Trade GetTradeFromEntity(TradeEntity entity)
        {
            if (entity is null)
            {
                return null;
            }
            
            var card = _cardRepository.Get(entity.CardId);
            var seller = _userRepository.GetById(entity.UserId);
            var tradeCreation = Trade.Create(entity.Id, entity.Type, entity.MinimumDamage, card, seller);
            return tradeCreation.Value;
        }
    }
}