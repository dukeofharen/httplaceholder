using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Hubs;
using HttPlaceholder.Web.Shared.HostedServices;
using HttPlaceholder.Web.Shared.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder;

/// <summary>
///     The HttPlaceholder startup class.
/// </summary>
public class Startup
{
    /// <summary>
    ///     Constructs a <see cref="Startup" /> instance.
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    /// <summary>
    ///     Configures the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public void ConfigureServices(IServiceCollection services) =>
        ConfigureServicesStatic(services, Configuration);

    /// <summary>
    ///     Configures the web application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web host environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
        ConfigureStatic(app, true, Configuration?.Get<SettingsModel>());

    /// <summary>
    ///     Configures the web application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="preloadStubs">Whether to preload the stubs or not.</param>
    /// <param name="settings">The HttPlaceholder settings.</param>
    public static void ConfigureStatic(IApplicationBuilder app, bool preloadStubs, SettingsModel settings) =>
        app.Configure(preloadStubs, settings)
            .UseEndpoints(options => options
                .ConfigureSignalR()
                .MapControllers());

    /// <summary>
    ///     Configures the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void ConfigureServicesStatic(IServiceCollection services, IConfiguration configuration) =>
        services
            .ConfigureServices<Startup>(configuration)
            .AddSignalRHubs()
            .AddHostedServices(configuration);
}
