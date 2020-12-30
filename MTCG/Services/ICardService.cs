using System;
using System.Collections.Generic;
using MTCG.Domain.Cards;
using MTCG.Results;
using MTCG.Results.Errors;

namespace MTCG.Services
{
    public interface ICardService
    {
        public Result<Card> GetCard(Guid cardId);

        public List<Card> GetAllCards();
    }
}