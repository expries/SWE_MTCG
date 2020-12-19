namespace MTCG.ActionResult.Errors
{
    public class NotFound : Error
    {
        public NotFound(string message) : base(message)
        {
        }
        
        public NotFound() : base("Card not found")
        {
        }
    }
}