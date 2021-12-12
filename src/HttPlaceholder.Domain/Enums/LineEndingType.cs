namespace HttPlaceholder.Domain.Enums;

/// <summary>
/// Specifies the line ending type for a response.
/// </summary>
public enum LineEndingType
{
    /// <summary>
    /// Not set.
    /// </summary>
    NotSet,

    /// <summary>
    /// Use Windows line endings (CR/NL).
    /// </summary>
    Windows,

    /// <summary>
    /// Use Unix line endings (NL).
    /// </summary>
    Unix
}