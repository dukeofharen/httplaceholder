using System.IO;
using System.Threading.Tasks;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Async;
using Ducode.Essentials.Files;
using Ducode.Essentials.Mvc;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Formatters;
using HttPlaceholder.Hubs;
using HttPlaceholder.Middleware;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder
{
    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesStatic(services);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ConfigureStatic(app, env, true, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="preloadStubs"></param>
        /// <param name="loadStaticFiles"></param>
        public static void ConfigureStatic(IApplicationBuilder app, IHostingEnvironment env, bool preloadStubs, bool loadStaticFiles)
        {
            app
                .UseSignalR(routes =>
                {
                    routes.MapHub<RequestHub>("/requestHub");
                })
               .UseMiddleware<ApiHeadersMiddleware>()
               .UseMiddleware<ApiExceptionHandlingMiddleware>()
               .UseMiddleware<StubHandlingMiddleware>()
               .UseMvc()
               .UseSwagger()
               .UseSwaggerUi3();

            if (preloadStubs)
            {
                // Check if the stubs can be loaded.
                var stubContainer = app.ApplicationServices.GetService<IStubContainer>();
                Task.Run(() => stubContainer.PrepareAsync()).GetAwaiter().GetResult();
            }

            if (loadStaticFiles)
            {
                string path = $"{AssemblyHelper.GetCallingAssemblyRootPath()}/gui";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServicesStatic(IServiceCollection services)
        {
            services
               .AddMvc(options =>
               {
                   options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
                   options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(namingConvention: new CamelCaseNamingConvention()).Build()));
                   options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
               })
               .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
               .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            services
                .AddSignalR();

            services
               .AddBusinessLogic()
               .AddUtilities()
               .AddHttpContextAccessor()
               .AddLogging()
               .AddDataLogic()
               .AddStubSources()
               .AddAssemblyServices()
               .AddCustomMvcServices()
               .AddFileServices()
               .AddAsyncServices();

            services.AddOpenApiDocument(c => c.Title = "HttPlaceholder API");
        }
    }
}