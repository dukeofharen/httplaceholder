using HttPlaceholder.Application.Infrastructure.Newtonsoft;
using HttPlaceholder.Hubs.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Hubs;

/// <summary>
/// A class for registering SignalR related classes on the service collection.
/// </summary>
public static class SignalRModule
{
    /// <summary>
    /// Registers SignalR related classes on the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddNewtonsoftJsonProtocol(options =>
                options.PayloadSerializerSettings.ContractResolver = new CamelCaseExceptDictionaryKeysResolver());
        services.AddTransient<IRequestNotify, RequestNotify>();
        return services;
    }
}
