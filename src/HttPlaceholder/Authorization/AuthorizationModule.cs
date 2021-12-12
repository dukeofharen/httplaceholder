using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Authorization.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Authorization;

public static class AuthorizationModule
{
    public static IServiceCollection AddAuthorizationModule(this IServiceCollection services)
    {
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IUserContext, UserContext>();
        return services;
    }
}