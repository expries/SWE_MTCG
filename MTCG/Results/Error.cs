using System.Linq;
using Newtonsoft.Json;

namespace MTCG.ActionResult
{
    public class Error
    {
        [JsonProperty("Error")]
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }
    }
}