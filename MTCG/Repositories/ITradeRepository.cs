using System;
using System.Collections.Generic;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface ITradeRepository
    {
        public List<Trade> GetAllTrades();
        
        public Trade GetTrade(Guid tradeId);

        public Trade CreateTrade(Trade trade);

        public void DeleteTrade(Trade trade);
    }
}