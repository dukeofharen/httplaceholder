using System;

namespace Placeholder.Exceptions
{
    public class RequestValidationException : Exception
    {
       public RequestValidationException(string message) : base(message)
       {
       }
    }
}
