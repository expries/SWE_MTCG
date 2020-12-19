namespace MTCG.ActionResult.Errors
{
    public class AllPackagesAcquired : Error
    {
        public AllPackagesAcquired(string message) : base(message)
        {
        }
        
        public AllPackagesAcquired() : base("All packages acquired.")
        {
        }
    }
}