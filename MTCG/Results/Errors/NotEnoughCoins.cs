namespace MTCG.ActionResult.Errors
{
    public class NotEnoughCoins : Error
    {
        public NotEnoughCoins(string message) : base(message)
        {
        }

        public NotEnoughCoins() : base("Not enough coins")
        {
            
        }
    }
}