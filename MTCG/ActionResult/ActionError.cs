using System.Linq;
using Newtonsoft.Json;

namespace MTCG.ActionResult
{
    public class ActionError
    {
        [JsonIgnore] 
        private readonly ServiceError _error;
        
        [JsonProperty("Error")]
        public string Message { get; }

        public ActionError(ServiceError error, string message) : this(message)
        {
            _error = error;
        }

        public ActionError(string message)
        {
            _error = default;
            Message = message;
        }

        public bool Equals(ServiceError error)
        {
            return _error.Equals(error);
        }

        public bool IsOneOf(params ServiceError[] errors)
        {
            return errors.Any(Equals);
        }
    }
}