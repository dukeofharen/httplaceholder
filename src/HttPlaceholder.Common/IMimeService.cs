namespace HttPlaceholder.Common;

/// <summary>
///     Describes a class that is used to convert between file extensions and mime types.
/// </summary>
public interface IMimeService
{
    /// <summary>
    ///     Converts a given input to a MIME type.
    /// </summary>
    /// <param name="input">The input (can be file path, file name or just plain extension).</param>
    /// <returns>The MIME type.</returns>
    string GetMimeType(string input);
}
