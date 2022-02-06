using HttPlaceholder.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace HttPlaceholder.HostedServices;

/// <summary>
/// A class for registering all hosted services.
/// </summary>
public static class HostedServiceModule
{
    /// <summary>
    /// A method for registering all hosted services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddHostedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.Get<SettingsModel>();
        if (settings?.Storage?.CleanOldRequestsInBackgroundJob == true)
        {
            services.AddHostedService<CleanOldRequestsJob>();
        }

        return services;
    }
}
