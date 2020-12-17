using System.Linq;
using Newtonsoft.Json;

namespace MTCG.ActionResult
{
    public class ActionError
    {
        [JsonIgnore]
        public ServiceError Error;
        
        [JsonProperty("Error")]
        public string Message { get; }

        public ActionError(ServiceError error, string message) : this(message)
        {
            Error = error;
        }

        public ActionError(string message)
        {
            Error = default;
            Message = message;
        }

        public bool Equals(ServiceError error)
        {
            return Error.Equals(error);
        }

        public bool IsOneOf(params ServiceError[] errors)
        {
            return errors.Any(Equals);
        }
    }
}