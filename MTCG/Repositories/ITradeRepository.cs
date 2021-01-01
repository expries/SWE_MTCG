using System;
using System.Collections.Generic;
using MTCG.Domain;

namespace MTCG.Repositories
{
    public interface ITradeRepository
    {
        public List<Trade> GetAll();
        
        public Trade Get(Guid tradeId);

        public Trade Create(Trade trade);

        public void Delete(Trade trade);
    }
}