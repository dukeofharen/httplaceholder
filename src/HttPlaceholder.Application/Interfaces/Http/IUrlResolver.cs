namespace HttPlaceholder.Application.Interfaces.Http;

/// <summary>
///     Describes a class that is used to resolve the HttPlaceholder (root) URLs.
/// </summary>
public interface IUrlResolver
{
    /// <summary>
    ///     Gets the current display URL (the full URL + protocol and hostname).
    /// </summary>
    /// <returns>The display URL.</returns>
    string GetDisplayUrl();

    /// <summary>
    ///     Gets the current root URL (protocol + hostname).
    /// </summary>
    /// <returns>The root URL.</returns>
    string GetRootUrl();
}
