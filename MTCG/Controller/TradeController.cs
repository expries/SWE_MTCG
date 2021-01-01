using System;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results.Errors;
using MTCG.Server;

namespace MTCG.Controller
{
    public class TradeController : ApiController
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;

        public TradeController(ITradeRepository tradeRepository, IUserRepository userRepository, ICardRepository cardRepository)
        {
            _tradeRepository = tradeRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
        }

        public ResponseContext GetTrades(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            if (_userRepository.GetByToken(token) is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }

            var trades = _tradeRepository.GetAll();
            return Ok(trades);
        }

        public ResponseContext CreateTrade(string token, TradeCreationRequest request)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }

            if (_tradeRepository.Get(request.Id) != null)
            {
                return Conflict("There already exists a trade with id " + request.Id + ".");
            }

            var card = _cardRepository.Get(request.CardToTrade);

            if (card is null)
            {
                return NotFound("Found no card with id " + request.CardToTrade + ".");
            }

            var createTrade = Trade.Create(request.Id, request.Type, request.MinimumDamage, card, user);

            if (createTrade.Success)
            {
                var trade = createTrade.Value;
                var newTrade = _tradeRepository.Create(trade);
                return Ok(newTrade);
            }

            if (createTrade.HasError<SellerDoesNotOwnCard>())
            {
                return Conflict(createTrade.Error);
            }

            return BadRequest(createTrade.Error);
        }
        
        public ResponseContext CommitTrade(string token, Guid tradeId, Guid cardId)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new {Error = "Authorization is required."});
            }

            var user = _userRepository.GetByToken(token);

            if (user is null)
            {
                return Forbidden(new {Error = "No user with this token exists."});
            }
            
            var trade = _tradeRepository.Get(tradeId);

            if (trade is null)
            {
                return NotFound(new {Error = "There exists no trade with this id."});
            }

            var card = _cardRepository.Get(cardId);

            if (card is null)
            {
                return NotFound(new {Error = "There exists no card with this id."});
            }

            var commitTrade = trade.Commit(user, card);

            if (!commitTrade.Success)
            {
                return Conflict(commitTrade.Error);
            }

            _userRepository.Update(user);
            _userRepository.Update(trade.Seller);
            return Ok(trade);
        }
    }
}