using System;
using System.Collections.Generic;
using MTCG.ActionResult;
using MTCG.ActionResult.Errors;
using MTCG.Resource.Cards;

namespace MTCG.Service
{
    public interface ICardService
    {
        public OneOf<Card, NotFound> GetCard(Guid cardId);

        public List<Card> GetAllCards();
    }
}