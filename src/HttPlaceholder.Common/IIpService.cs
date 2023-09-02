namespace HttPlaceholder.Common;

/// <summary>
///     Describes a service for working with IP addresses.
/// </summary>
public interface IIpService
{
    /// <summary>
    ///     This method returns the local IP address of the current machine, or null if it could not be found.
    /// </summary>
    /// <returns>The local IP address, or null if it was not found.</returns>
    string GetLocalIpAddress();
}
