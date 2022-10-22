using System;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Client;

/// <summary>
///     A static class for registering the HttPlaceholder client on your DI container.
/// </summary>
/// <example>
///     Use the following code to add the client to your .NET Core application.
///     <code>
/// ...
/// public void ConfigureServices(IServiceCollection services)
/// {
/// services.AddHttPlaceholderClient(config =&gt;
/// {
/// config.RootUrl = "http://localhost:5000/"; // The HttPlaceholder root URL.
/// config.Username = "username"; // Optionally set the authentication.
/// config.Password = "password";
/// })
/// }
/// ...
/// </code>
///     Now you can inject an object of type <see cref="IHttPlaceholderClient" /> in your class of choice and call the
///     HttPlaceholder API.
/// </example>
public static class HttPlaceholderClientModule
{
    /// <summary>
    ///     Adds the <see cref="HttPlaceholderClient" /> to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection the client should be added to.</param>
    /// <param name="clientConfigSection">The section that contains the configuration for the HttPlaceholder configuration.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    public static IServiceCollection AddHttPlaceholderClient(
        this IServiceCollection serviceCollection,
        IConfiguration clientConfigSection)
    {
        serviceCollection.Configure<HttPlaceholderClientConfiguration>(clientConfigSection);
        var config = clientConfigSection.Get<HttPlaceholderClientConfiguration>();
        config.Validate();
        return serviceCollection.RegisterHttpClient(config);
    }

    /// <summary>
    ///     Adds the <see cref="HttPlaceholderClient" /> to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection the client should be added to.</param>
    /// <param name="configAction">An action to configure the client.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    public static IServiceCollection AddHttPlaceholderClient(
        this IServiceCollection serviceCollection,
        Action<HttPlaceholderClientConfiguration> configAction = null)
    {
        var config = new HttPlaceholderClientConfiguration();
        configAction?.Invoke(config);
        config.Validate();
        return serviceCollection.RegisterHttpClient(config);
    }

    private static IServiceCollection RegisterHttpClient(
        this IServiceCollection serviceCollection,
        HttPlaceholderClientConfiguration config)
    {
        serviceCollection.AddHttpClient<IHttPlaceholderClient, HttPlaceholderClient>(client =>
            client.ApplyConfiguration(config));
        return serviceCollection;
    }
}
