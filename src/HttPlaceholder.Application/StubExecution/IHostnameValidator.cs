namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to check whether a certain hostname is usable for the reverse proxy response writer or not.
/// </summary>
public interface IHostnameValidator
{
    /// <summary>
    ///     Checks whether the given hostname / IP address is valid.
    /// </summary>
    /// <param name="hostname">The hostname.</param>
    /// <returns>True if the hostname is valid.</returns>
    bool HostnameIsValid(string hostname);
}
