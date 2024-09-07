using System;
using System.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class used for working with bytes and byte arrays.
/// </summary>
public static class ByteUtilities
{
    /// <summary>
    ///     Checks whether the source is a valid ASCII string.
    /// </summary>
    /// <param name="source">The byte array.</param>
    /// <returns>True if the source is a valid ASCII string; false otherwise.</returns>
    public static bool IsValidAscii(this byte[] source) => source == null || source.All(b => b <= 127);

    /// <summary>
    ///     Checks whether the source byte array is a valid string that can be mutated using string replace for example.
    ///     Also takes the MIME content type header into consideration.
    /// </summary>
    /// <param name="source">The byte array.</param>
    /// <param name="contentType">The MIME content header.</param>
    /// <returns>True if the source is a valid string. False otherwise.</returns>
    public static bool IsValidText(this byte[] source, string contentType) =>
        (!string.IsNullOrWhiteSpace(contentType) &&
         contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)) ||
        source.IsValidAscii();
}
