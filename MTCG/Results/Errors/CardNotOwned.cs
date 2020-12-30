namespace MTCG.Results.Errors
{
    public class CardNotOwned : Error
    {
        public CardNotOwned(string message) : base(message)
        {
        }
    }
}