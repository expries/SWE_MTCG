using System;
using System.Collections.Generic;
using MTCG.Domain;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public interface ITradeService
    {
        public Result<List<Trade>> GetTrades(string token);

        public Result<Trade> CreateTrade(string seller, TradeCreationRequest trade);
        
        public Result<Trade> CommitTrade(Guid tradeId, Guid cardId, string token);
    }
}