using System;

namespace HttPlaceholder.Application.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message) : base(message)
        {
        }
    }
}
