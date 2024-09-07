using System;

namespace HttPlaceholder.Application.Exceptions;

/// <summary>
///     An exception that is thrown when a specific item could not be found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    ///     Constructs a <see cref="NotFoundException" /> instance.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    /// <param name="key">The key of the item.</param>
    public NotFoundException(string name, object key)
        : base(string.Format(ApplicationResources.EntityNotFound, name, key))
    {
    }

    /// <summary>
    ///     Constructs a <see cref="NotFoundException" /> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Throws an exception if the input is null.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="key">The key of the item.</param>
    /// <exception cref="NotFoundException">The exception.</exception>
    public static void ThrowIfNull(object input, string name, object key)
    {
        if (input == null)
        {
            throw new NotFoundException(name, key);
        }
    }
}
