using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.IntegrationTests.Clients;

public static class ClientsModule
{
    public static IServiceCollection AddClientsModule(this IServiceCollection services) =>
        services.AddTransient<DevelopmentClient>();
}
