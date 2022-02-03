using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Authorization.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Authorization;

/// <summary>
/// A class that registers the necessary Authorization classes to the service collection.
/// </summary>
public static class AuthorizationModule
{
    /// <summary>
    /// Registers the necessary Authorization classes to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddAuthorizationModule(this IServiceCollection services) =>
        services
            .AddTransient<ILoginService, LoginService>()
            .AddTransient<IUserContext, UserContext>();
}
