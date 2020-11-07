using System;

namespace MTCG.Exceptions
{
    public class MethodNotImplementedException : Exception
    {
        public MethodNotImplementedException() {}

        public MethodNotImplementedException(string message)
            : base(message) {}

        public MethodNotImplementedException(string message, Exception inner)
            : base(message, inner) {}
    }
}