using System.Net.Http;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application;

/// <summary>
///     A class for registering all classes in the Application project.
/// </summary>
public static class ApplicationModule
{
    /// <summary>
    ///     Register all classes in the Application project.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));

        // Register implementations
        services.Scan(scan => scan.FromCallingAssembly().RegisterDependencies());

        // Add other modules
        services.AddHttpClient("proxy")
            .ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });

        return services;
    }
}
