using Newtonsoft.Json;

namespace MTCG.Results
{
    public class Error
    {
        [JsonProperty("Error")]
        public string Message { get; protected set; }

        public Error(string message)
        {
            Message = message;
        }
    }
}