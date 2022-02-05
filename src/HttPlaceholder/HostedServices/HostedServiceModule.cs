using Microsoft.Extensions.DependencyInjection;

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
    public static IServiceCollection AddHostedServices(this IServiceCollection services) => services;
}
