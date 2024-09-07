using System;

namespace HttPlaceholder.Application.Exceptions;

/// <summary>
///     An exception that is thrown when an action conflicts with already existing data.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    ///     Constructs a <see cref="ConflictException" /> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    public ConflictException(string message) : base(string.Format(ApplicationResources.ConflictDetected, message))
    {
    }
}
