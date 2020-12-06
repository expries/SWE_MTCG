namespace MTCG.ActionResults
{
    public class ActionResult<TMember>
    {
        public TMember Item { get; }
        
        public ActionError Error { get; }

        public bool Success => Error is null;

        public ActionResult()
        {
            Error = null;
        }

        public ActionResult(TMember item)
        {
            Item = item;
        }

        public ActionResult(string message)
        {
            Error = new ActionError(message);
        }

        public ActionResult(ServiceError error, string message)
        {
            Error = new ActionError(error, message);
        }
    }
}