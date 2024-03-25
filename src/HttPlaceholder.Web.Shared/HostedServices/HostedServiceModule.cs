using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;

namespace HttPlaceholder.Web.Shared.HostedServices;

/// <summary>
///     A class for registering all hosted services.
/// </summary>
public static class HostedServiceModule
{
    /// <summary>
    ///     A method for registering all hosted services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddHostedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.Get<SettingsModel>();
        services.AddHostedService<LifetimeEventsHostedService>();
        if (settings?.Storage?.CleanOldRequestsInBackgroundJob == true)
        {
            services.RegisterCustomHostedService<CleanOldRequestsJob>();
        }

        return services;
    }
}
