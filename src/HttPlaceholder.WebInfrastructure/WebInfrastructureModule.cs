using HttPlaceholder.Application.Infrastructure.DependencyInjection;

namespace HttPlaceholder.WebInfrastructure;

/// <summary>
///     A static class to register dependencies in the WebInfrastructure project.
/// </summary>
public static class WebInfrastructureModule
{
    /// <summary>
    ///     Registers all calsses in the WebInfrastructure module on the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddWebInfrastructureModule(this IServiceCollection services) =>
        services
            .Scan(scan => scan.FromCallingAssembly().RegisterDependencies());
}
