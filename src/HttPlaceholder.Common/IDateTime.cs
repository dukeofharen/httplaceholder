using System;

namespace HttPlaceholder.Common;

/// <summary>
///     Describes a class that is used to work with <see cref="DateTime" /> instances.
/// </summary>
public interface IDateTime
{
    /// <summary>
    ///     Gets the current local date and time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    ///     Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    ///     Gets the current UTC date and time as UNIX timestamp milliseconds.
    /// </summary>
    long UtcNowUnix { get; }
}
