using HttPlaceholder.Application.Infrastructure.Newtonsoft;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Hubs.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Hubs;

/// <summary>
///     A class for registering SignalR related classes on the service collection.
/// </summary>
public static class SignalRModule
{
    /// <summary>
    ///     Registers SignalR related classes on the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddSignalRHubs(this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddNewtonsoftJsonProtocol(options =>
                options.PayloadSerializerSettings.ContractResolver = new CamelCaseExceptDictionaryKeysResolver());
        services.AddTransient<IRequestNotify, RequestNotify>();
        services.AddTransient<IScenarioNotify, ScenarioNotify>();
        return services;
    }

    /// <summary>
    ///     Configures SignalR.
    /// </summary>
    /// <param name="options">The endpoint route builder options.</param>
    public static IEndpointRouteBuilder ConfigureSignalR(this IEndpointRouteBuilder options)
    {
        options.MapHub<RequestHub>("/requestHub");
        options.MapHub<ScenarioHub>("/scenarioHub");
        return options;
    }
}
