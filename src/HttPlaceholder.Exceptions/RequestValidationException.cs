using System;

namespace HttPlaceholder.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message) : base(message)
        {
        }
    }
}