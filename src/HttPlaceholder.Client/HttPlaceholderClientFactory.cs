using System.Net.Http;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.Utilities;

namespace HttPlaceholder.Client;

/// <summary>
/// A class for creating instances of <see cref="HttPlaceholderClient"/>.
/// Useful for projects that do not use .NET Core or the .NET Core DI container.
/// </summary>
/// <example>
/// If you do not use .NET Core or the .NET Core dependency container, use this method to create a new client.
///
/// <code>
///...
/// var config = new HttPlaceholderClientConfiguration
/// {
/// RootUrl = "http://localhost:5000", // The HttPlaceholder root URL.
/// Username = "username", // Optionally set the authentication.
/// Password = "password"
/// };
/// var client = HttPlaceholderClientFactory.CreateHttPlaceholderClient(config);
/// ...
/// </code>
/// </example>
public static class HttPlaceholderClientFactory
{
    // Static HttpClient that will be used for the client.
    private static HttpClient _httpClient;

    /// <summary>
    /// Method for creating the <see cref="IHttPlaceholderClient"/>.
    /// </summary>
    /// <param name="config">The configuration of the client.</param>
    /// <returns>The <see cref="IHttPlaceholderClient"/>.</returns>
    public static IHttPlaceholderClient CreateHttPlaceholderClient(HttPlaceholderClientConfiguration config)
    {
        config.Validate();
        if (_httpClient == null)
        {
            _httpClient = new HttpClient();
            _httpClient.ApplyConfiguration(config);
        }

        return new HttPlaceholderClient(_httpClient);
    }
}