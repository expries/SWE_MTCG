namespace MTCG.ActionResult.Errors
{
    public class PackageIsEmpty : Error
    {
        public PackageIsEmpty(string message) : base(message)
        {
        }

        public PackageIsEmpty() : base ("Package is empty.")
        {
            
        }
    }
}