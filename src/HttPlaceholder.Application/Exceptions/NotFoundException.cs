using System;

namespace HttPlaceholder.Application.Exceptions;

/// <summary>
/// An exception that is thrown when a specific item could not be found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Constructs a <see cref="NotFoundException"/> instance.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="key">The key of the item.</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }

    /// <summary>
    /// Constructs a <see cref="NotFoundException"/> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    public NotFoundException(string message) : base(message)
    {
    }
}
