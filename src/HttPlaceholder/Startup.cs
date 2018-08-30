using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.Formatters;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Middleware;
using HttPlaceholder.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HttPlaceholder.Utilities;
using System.IO;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace HttPlaceholder
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         ConfigureServicesStatic(services);
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         ConfigureStatic(app, env, true, true);
      }

      public static void ConfigureStatic(IApplicationBuilder app, IHostingEnvironment env, bool preloadStubs, bool loadStaticFiles)
      {
         app.UseMiddleware<StubHandlingMiddleware>();
         app.UseMiddleware<CorsHeadersMiddleware>();
         app.UseMvc();
         app.UseSwagger();
         app.UseSwaggerUI(c =>
         {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "HttPlaceholder API V1");
         });

         if (preloadStubs)
         {
            // Check if the stubs can be loaded.
            var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
            Task.Run(() => stubContainer.GetStubsAsync()).Wait();
         }

         if (loadStaticFiles)
         {
            string path = $"{AssemblyHelper.GetExecutingAssemblyRootPath()}/dist";
            if (Directory.Exists(path))
            {
               app.UseFileServer(new FileServerOptions
               {
                  EnableDefaultFiles = true,
                  FileProvider = new PhysicalFileProvider(path),
                  RequestPath = "/ph-ui"
               });
            }
         }
      }

      public static void ConfigureServicesStatic(IServiceCollection services)
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
   }
}
