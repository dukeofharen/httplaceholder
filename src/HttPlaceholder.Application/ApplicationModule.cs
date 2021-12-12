using System.Net.Http;
using HttPlaceholder.Application.StubExecution;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application;

public static class ApplicationModule
{
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