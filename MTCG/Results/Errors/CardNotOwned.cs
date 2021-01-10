using MTCG.Results;

namespace MTCG.ActionResult.Errors
{
    public class CardNotOwned : Error
    {
        public CardNotOwned(string message) : base(message)
        {
        }

        public CardNotOwned() : base ("Card not owned.")
        {
            
        }
    }
}