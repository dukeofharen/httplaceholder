using System.Net.Http;
using HttPlaceholder.Application.StubExecution;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application;

/// <summary>
/// A class for registering all classes in the Application project.
/// </summary>
public static class ApplicationModule
{
    /// <summary>
    /// Register all classes in the Application project.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
        var currentAssembly = typeof(ApplicationModule).Assembly;

        // Add MediatR
        services.AddMediatR(currentAssembly);

        // Add other modules
        services.AddStubExecutionModule();
        services.AddHttpClient("proxy").ConfigureHttpMessageHandlerBuilder(h =>
            h.PrimaryHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

        return services;
    }
}
