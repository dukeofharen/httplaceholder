using System;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A small utility class for helping with exceptions.
/// </summary>
public static class ThrowHelper
{
    /// <summary>
    ///     Checks if item is null. If so, an exception is thrown.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="message"></param>
    /// <returns>The provided item.</returns>
    public static T ThrowIfNull<T, TException>(T item, string message = "") where TException : Exception
    {
        if (item != null)
        {
            return item;
        }

        var exception = (TException)(string.IsNullOrWhiteSpace(message)
            ? Activator.CreateInstance(typeof(TException))
            : Activator.CreateInstance(typeof(TException), message));
        throw exception;
    }
}
