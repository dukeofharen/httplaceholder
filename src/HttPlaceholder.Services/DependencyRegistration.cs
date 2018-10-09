using HttPlaceholder.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttPlaceholder.Services
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.TryAddTransient<IAssemblyService, AssemblyService>();
            services.TryAddTransient<IAsyncService, AsyncService>();
            services.TryAddTransient<IConfigurationService, ConfigurationService>();
            services.TryAddTransient<IFileService, FileService>();
            services.TryAddTransient<IHttpContextService, HttpContextService>();
            services.TryAddTransient<IRequestLoggerFactory, RequestLoggerFactory>();
            services.TryAddTransient<IWebService, WebService>();
            services.TryAddTransient<IYamlService, YamlService>();
            return services;
        }
    }
}