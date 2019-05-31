using HttPlaceholder.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
        {
            services.AddSingleton<IDateTime, MachineDateTime>();
            return services;
        }
    }
}
