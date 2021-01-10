using System;

namespace MTCG.Requests
{
    public class MessageCreationRequest
    {
        public Guid Id { get; set; }
        
        public string Receiver { get; set; }

        public string Message { get; set; }
    }
}