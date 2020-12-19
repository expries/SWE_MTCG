namespace MTCG.ActionResult.Errors
{
    public class BadFormat : Error
    {
        public BadFormat(string message) : base(message)
        {
        }

        public BadFormat() : base("Bad format.")
        {
            
        }
    }
}