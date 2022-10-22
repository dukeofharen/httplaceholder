using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HttPlaceholder.Common;

/// <summary>
///     Describes a class that is used to work with environments.
/// </summary>
public interface IEnvService
{
    /// <summary>
    ///     Returns a dictionary containing environment variables.
    /// </summary>
    /// <returns>A dictionary containing the environment variables.</returns>
    IDictionary<string, string> GetEnvironmentVariables();

    /// <summary>
    ///     Retrieves a specific environment variable.
    /// </summary>
    /// <param name="key">The environment variable to look for.</param>
    /// <returns>The environment variable value.</returns>
    string GetEnvironmentVariable(string key);

    /// <summary>
    ///     Checks whether the current OS is a specific OS.
    /// </summary>
    /// <param name="platform">The OS to check for.</param>
    /// <returns>True if the current OS is the provided OS, false otherwise.</returns>
    bool IsOs(OSPlatform platform);
}
