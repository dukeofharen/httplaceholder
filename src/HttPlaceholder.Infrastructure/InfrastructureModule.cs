using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Infrastructure;

/// <summary>
///     A module for registering all classes in the Infrastructure project to the service collection.
/// </summary>
public static class InfrastructureModule
{
    /// <summary>
    ///     Register all classes in the Infrastructure project to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services) =>
        services.Scan(scan => scan.FromCallingAssembly().RegisterDependencies());
}
