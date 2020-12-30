using System;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;
using MTCG.Services;

namespace MTCG.Controller
{
    public class TradeController : ApiController
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        public ResponseContext GetTrades(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }
            
            var result = _tradeService.GetTrades(token);

            if (!result.Success)
            {
                return NotFound(result.Error);
            }

            var trades = result.Value;
            return Ok(trades);
        }

        public ResponseContext CreateTrade(string username, TradeCreationRequest request)
        {
            var result = _tradeService.CreateTrade(username, request);

            if (result.Success)
            {
                var trade = result.Value;
                return Ok(trade);
            }

            if (result.HasError<UserNotFound, CardNotFound>())
            {
                return NotFound(result.Error);
            }

            if (result.HasError<DuplicateTradeId>())
            {
                return Conflict(result.Error);
            }
            
            return BadRequest(result.Error);
        }
        
        public ResponseContext CommitTrade(Guid tradeId, Guid cardId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new {Error = "Authorization is required."});
            }
            
            var result = _tradeService.CommitTrade(tradeId, cardId, token);
            
            if (result.Success)
            {
                var trade = result.Value;
                return Ok(trade);
            }

            if (result.HasError<TradeNotFound, CardNotFound, UserNotFound>())
            {
                return NotFound(result.Error);
            }

            if (result.HasError<CardNotOwned, TooLowDamage, IncorrectCardType>())
            {
                return Conflict(result.Error);
            }
            
            return BadRequest(result.Error);
        }
    }
}