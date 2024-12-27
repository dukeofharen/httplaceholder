using System;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A simple utility class for working with URLs.
/// </summary>
public static class UrlHelper
{
    /// <summary>
    ///     Returns the hostname of the given URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The extracted hostname.</returns>
    public static string GetHostname(string url)
    {
        var uri = new Uri(url);
        return uri.Host;
    }
}
