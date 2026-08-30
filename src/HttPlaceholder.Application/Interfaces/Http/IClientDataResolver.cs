namespace HttPlaceholder.Application.Interfaces.Http;

/// <summary>
///     Describes a class that is used to retrieve data about the calling client.
/// </summary>
public interface IClientDataResolver
{
    /// <summary>
    ///     Gets the calling client IP.
    /// </summary>
    /// <returns>The calling client IP.</returns>
    string GetClientIp();

    /// <summary>
    ///     Gets the hostname of the request.
    /// </summary>
    /// <returns>The hostname of the request.</returns>
    string GetHost();

    /// <summary>
    ///     Gets whether the request was made over HTTPS.
    /// </summary>
    /// <returns>True if the request was made over HTTPS, false otherwise.</returns>
    bool IsHttps();
}
