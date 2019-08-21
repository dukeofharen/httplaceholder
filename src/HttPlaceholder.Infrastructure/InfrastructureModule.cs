using HttPlaceholder.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttPlaceholder.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
        {
            services.TryAddSingleton<IDateTime, MachineDateTime>();
            return services;
        }
    }
}
