using System;
using System.Collections.Generic;
using System.Linq;
using MTCG.Domain;
using MTCG.Repositories;
using MTCG.Requests;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        
        public TradeService(ITradeRepository tradeRepository, IUserRepository userRepository, ICardRepository cardRepository)
        {
            _tradeRepository = tradeRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
        }

        public Result<List<Trade>> GetTrades(string token)
        {
            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new UserNotFound("No user with this token exists.");
            }
            
            return _tradeRepository.GetAllTrades();
        }

        public Result<Trade> CreateTrade(string seller, TradeCreationRequest tradeRequest)
        {
            var existentTrade = _tradeRepository.GetTrade(tradeRequest.Id);

            if (existentTrade != null)
            {
                return new DuplicateTradeId("There exists another trade with this ID.");
            }
            
            var user = _userRepository.GetUserByUsername(seller);

            if (user is null)
            {
                return new UserNotFound("There is no user with this ID.");
            }
            
            var card = _cardRepository.GetCard(tradeRequest.CardToTrade);

            if (card is null)
            {
                return new CardNotFound("There is no card with this ID.");
            }

            bool userOwnsCard = user.Stack.Select(x => x.Id).Contains(tradeRequest.CardToTrade);

            if (!userOwnsCard)
            {
                return new CardNotOwned("You do not own the card with id" + tradeRequest.CardToTrade);
            }
            
            var createTrade = 
                Trade.Create(tradeRequest.Id, tradeRequest.Type, tradeRequest.MinimumDamage, card, user);

            if (!createTrade.Success)
            {
                return new BadTrade(createTrade.Error.Message);
            }

            var trade = createTrade.Value;
            var createdTrade = _tradeRepository.CreateTrade(trade);
            return createdTrade;
        }

        public Result<Trade> CommitTrade(Guid tradeId, Guid cardId, string token)
        {
            var trade = _tradeRepository.GetTrade(tradeId);

            if (trade is null)
            {
                return new TradeNotFound("There exists no trade with this id.");
            }

            var card = _cardRepository.GetCard(cardId);

            if (card is null)
            {
                return new CardNotFound("There exists no card with this id.");
            }

            var user = _userRepository.GetUserByToken(token);

            if (user is null)
            {
                return new UserNotFound("There exists no user with this id.");
            }

            var commitTrade = trade.Commit(user, card);

            if (!commitTrade.Success)
            {
                return new BadTrade(commitTrade.Error.Message);
            }

            _userRepository.UpdateUser(user);
            _userRepository.UpdateUser(trade.Seller);
            _tradeRepository.DeleteTrade(trade);
            return trade;
        }
    }
}