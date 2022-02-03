using System;

namespace HttPlaceholder.Application.Exceptions;

/// <summary>
/// An exception that is thrown when anything stub related went wrong.
/// </summary>
public class RequestValidationException : Exception
{
    /// <summary>
    /// Constructs a <see cref="RequestValidationException"/> instance.
    /// </summary>
    /// <param name="message"></param>
    public RequestValidationException(string message) : base(message)
    {
    }
}
