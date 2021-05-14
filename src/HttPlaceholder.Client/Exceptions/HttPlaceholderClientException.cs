using System;

namespace HttPlaceholder.Client.Exceptions
{
    public class HttPlaceholderClientException : Exception
    {
        public HttPlaceholderClientException(string message) : base(message)
        {
        }
    }
}
