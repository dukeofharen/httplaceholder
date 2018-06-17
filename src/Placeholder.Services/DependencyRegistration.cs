using Placeholder.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Placeholder.Services
{
   public static class DependencyRegistration
   {
      public static void RegisterDependencies(IServiceCollection services)
      {
         services.AddTransient<IAssemblyService, AssemblyService>();
         services.AddTransient<IAsyncService, AsyncService>();
         services.AddTransient<IConfigurationService, ConfigurationService>();
         services.AddTransient<IFileService, FileService>();
         services.AddTransient<IHttpContextService, HttpContextService>();
         services.AddTransient<IRequestLoggerFactory, RequestLoggerFactory>();
         services.AddTransient<IYamlService, YamlService>();
      }
   }
}
