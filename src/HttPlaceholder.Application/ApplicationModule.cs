using System.Net.Http;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Infrastructure.MediatR;
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
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly);
            cfg.AddOpenBehavior(typeof(AuditBehavior<,>));
        });

        // Register implementations
        services.Scan(scan => scan.FromCallingAssembly().RegisterDependencies());

        // Add other modules
        services.AddHttpClient("proxy").ConfigureHttpMessageHandlerBuilder(h =>
            h.PrimaryHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

        return services;
    }
}
