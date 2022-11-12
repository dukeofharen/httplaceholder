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
}
