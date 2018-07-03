using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.Formatters;
using HttPlaceholder.Implementation;
using HttPlaceholder.Middleware;
using HttPlaceholder.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder
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
         services.AddHttpContextAccessor();

         services.AddSwaggerGen(c =>
         {
            c.SwaggerDoc("v1", new Info { Title = "HttPlaceholder API", Version = "v1" });
            c.OperationFilter<StatusCodeOperationFilter>();
         });

         DependencyRegistration.RegisterDependencies(services);
         Services.DependencyRegistration.RegisterDependencies(services);
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         app.UseMiddleware<StubHandlingMiddleware>();
         app.UseMvc();
         app.UseSwagger();
         app.UseSwaggerUI(c =>
         {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "HttPlaceholder API V1");
         });

         // Check if the stubs can be loaded.
         var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
         Task.Run(() => stubContainer.GetStubsAsync()).Wait();
      }
   }
}
