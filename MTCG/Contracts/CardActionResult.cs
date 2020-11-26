namespace MTCG.Contracts
{
    public enum CardError
    {
        NotFound,
        
    }
    
    public class CardActionResult : ActionResult<CardError>
    {
        
    }
}