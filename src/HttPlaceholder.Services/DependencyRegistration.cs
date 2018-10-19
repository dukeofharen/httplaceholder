using HttPlaceholder.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttPlaceholder.Services
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.TryAddSingleton<IAssemblyService, AssemblyService>();
            services.TryAddSingleton<IAsyncService, AsyncService>();
            services.TryAddSingleton<IConfigurationService, ConfigurationService>();
            services.TryAddSingleton<IFileService, FileService>();
            services.TryAddSingleton<IHttpContextService, HttpContextService>();
            services.TryAddSingleton<IRequestLoggerFactory, RequestLoggerFactory>();
            services.TryAddSingleton<IYamlService, YamlService>();
            return services;
        }
    }
}