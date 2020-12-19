namespace MTCG.ActionResult.Errors
{
    public class UsernameIsTaken : Error
    {
        public UsernameIsTaken(string message) : base(message)
        {
        }

        public UsernameIsTaken() : base ("Username is taken.")
        {
            
        }
    }
}