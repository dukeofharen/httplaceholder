using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Placeholder.Formatters;
using Placeholder.Implementation;
using Placeholder.Middleware;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Placeholder
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMvc(options =>
         {
            options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
            options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
            options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
         });
         services.AddLogging();
         services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         DependencyRegistration.RegisterDependencies(services);
         Services.DependencyRegistration.RegisterDependencies(services);
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         app.UseMiddleware<StubHandlingMiddleware>();
         app.UseMvc();

         // Check if the stubs can be loaded.
         var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
         Task.Run(() => stubContainer.GetStubsAsync()).Wait();
      }
   }
}
