using System.IO;
using System.Threading.Tasks;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Mvc;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Formatters;
using HttPlaceholder.Middleware;
using HttPlaceholder.Services;
using HttPlaceholder.Swagger;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            app
               .UseMiddleware<ApiHeadersMiddleware>()
               .UseMiddleware<ApiExceptionHandlingMiddleware>()
               .UseMiddleware<StubHandlingMiddleware>()
               .UseMvc()
               .UseSwagger()
               .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HttPlaceholder API V1");
            });

            if (preloadStubs)
            {
                // Check if the stubs can be loaded.
                var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
                Task.Run(() => stubContainer.GetStubsAsync()).GetAwaiter().GetResult();
            }

            if (loadStaticFiles)
            {
                string path = $"{AssemblyHelper.GetExecutingAssemblyRootPath()}/gui";
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
            services
               .AddMvc(options =>
               {
                   options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
                   options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
                   options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
               })
               .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            services
               .AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("v1", new Info { Title = "HttPlaceholder API", Version = "v1" });
                   c.OperationFilter<StatusCodeOperationFilter>();
               })
               .AddBusinessLogic()
               .AddUtilities()
               .AddHttpContextAccessor()
               .AddLogging()
               .AddDataLogic()
               .AddStubSources()
               .AddAssemblyServices()
               .AddCustomMvcServices();
        }
    }
}