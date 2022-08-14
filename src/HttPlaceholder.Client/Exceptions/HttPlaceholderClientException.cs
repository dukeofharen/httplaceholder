using System;
using System.Net;

namespace HttPlaceholder.Client.Exceptions;

/// <summary>
/// An exception that is thrown when something went wrong when calling HttPlaceholder.
/// </summary>
public class HttPlaceholderClientException : Exception
{
    /// <summary>
    /// Creates a <see cref="HttPlaceholderClientException"/> instance.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="content">The returned content.</param>
    public HttPlaceholderClientException(HttpStatusCode statusCode, string content) : base(
        $"Status code '{(int)statusCode}' returned by HttPlaceholder with message '{content}'")
    {
    }
}