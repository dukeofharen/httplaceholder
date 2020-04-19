using HttPlaceholder.Common;
using HttPlaceholder.Infrastructure.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttPlaceholder.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection services)
        {
            services.TryAddSingleton<IAssemblyService, AssemblyService>();
            services.TryAddSingleton<IAsyncService, AsyncService>();
            services.TryAddSingleton<IDateTime, MachineDateTime>();
            services.TryAddSingleton<IEnvService, EnvService>();
            services.TryAddSingleton<IFileService, FileService>();
            return services;
        }
    }
}
