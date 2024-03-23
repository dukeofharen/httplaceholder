using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;

namespace HttPlaceholder.Web.Shared.Utilities;

/// <summary>
///     Describes a class that is used to help start up HttPlaceholder.
/// </summary>
public interface IProgramUtility
{
    /// <summary>
    ///     Parses the HTTP and HTTPS ports from the settings and returns both HTTP and HTTPS ports as a arrays.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <returns>A tuple containing lists with HTTP and HTTPS ports.</returns>
    (IEnumerable<int> httpPorts, IEnumerable<int> httpsPorts) GetPorts(SettingsModel settings);

    /// <summary>
    ///     Gets the hostnames and IP addresses the API is reachable on.
    /// </summary>
    /// <returns>A list of IP addresses.</returns>
    IEnumerable<string> GetHostnames();
}
