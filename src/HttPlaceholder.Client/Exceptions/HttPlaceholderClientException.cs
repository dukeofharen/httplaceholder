using System;
using System.Net;

namespace HttPlaceholder.Client.Exceptions
{
    public class HttPlaceholderClientException : Exception
    {
        public HttPlaceholderClientException(string message) : base(message)
        {
        }

        public HttPlaceholderClientException(HttpStatusCode statusCode, string content) : base(
            $"Status code '{(int)statusCode}' returned by HttPlaceholder with message '{content}'")
        {
        }
    }
}
