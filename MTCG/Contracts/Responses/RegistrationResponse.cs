using System.Collections.Generic;

namespace MTCG.Contracts.Responses
{
    public class RegistrationResponse
    {
        public string Error { get; set; }
        public string Token { get; set; }
        public bool Success { get; set; }
    }
}