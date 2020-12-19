namespace MTCG.ActionResult.Errors
{
    public class DuplicateId : Error
    {
        public DuplicateId(string message) : base(message)
        {
        }

        public DuplicateId() : base("ID is taken")
        {
        }
    }
}