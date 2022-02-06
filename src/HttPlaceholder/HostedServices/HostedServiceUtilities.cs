using HttPlaceholder.Controllers.v1;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.HostedServices;

/// <summary>
/// A utility class that is used for working with hosted services.
/// </summary>
public static class HostedServiceUtilities
{
    /// <summary>
    /// A method for registering a hosted service.
    /// It also registers an additional "transient" dependency on the service container so they can be used in the <see cref="ScheduledJobController"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <typeparam name="THostedService">The type of the hosted service.</typeparam>
    public static IServiceCollection RegisterCustomHostedService<THostedService>(this IServiceCollection services)
        where THostedService : class, ICustomHostedService =>
        services
            .AddHostedService<THostedService>()
            .AddTransient<ICustomHostedService, THostedService>();
}
