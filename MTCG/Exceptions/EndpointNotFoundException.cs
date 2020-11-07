using System;

namespace MTCG.Exceptions
{
    public class EndpointNotFoundException : Exception
    {
        public EndpointNotFoundException() {}

        public EndpointNotFoundException(string message)
            : base(message) {}

        public EndpointNotFoundException(string message, Exception inner)
            : base(message, inner) {}
    }
}