using MTCG.Results;

namespace MTCG.ActionResult.Errors
{
    public class DeckNotSet : Error
    {
        public DeckNotSet(string message) : base(message)
        {
        }

        public DeckNotSet() : base("Deck could not be set.")
        {
            
        }
    }
}