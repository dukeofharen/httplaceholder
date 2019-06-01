using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Async;
using Ducode.Essentials.Files;
using Ducode.Essentials.Mvc;
using HttPlaceholder.Application;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Authorization.Implementations;
using HttPlaceholder.Configuration;
using HttPlaceholder.Formatters;
using HttPlaceholder.Hubs;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.Middleware;
using HttPlaceholder.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            ConfigureServicesStatic(services, Configuration);

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
            ConfigureStatic(app, env, true, true);

        public static void ConfigureStatic(IApplicationBuilder app, IHostingEnvironment env, bool preloadStubs, bool loadStaticFiles)
        {
            app
               .UseSignalR(r => r.MapHub<RequestHub>("/requestHub"))
               .UseMiddleware<ApiHeadersMiddleware>()
               .UseMiddleware<ApiExceptionHandlingMiddleware>()
               .UseMiddleware<StubHandlingMiddleware>()
               .UseMvc()
               .UseSwagger()
               .UseSwaggerUi3();

            if (preloadStubs)
            {
                // Check if the stubs can be loaded.
                var stubContainer = app.ApplicationServices.GetService<IStubContext>();
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

        public static void ConfigureServicesStatic(IServiceCollection services, IConfiguration configuration)
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

            services.Configure<SettingsModel>(configuration);

            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IRequestNotify, RequestNotify>();
            services.AddTransient<IUserContext, UserContext>();

            services
               .AddApplicationModule()
               .AddHttpContextAccessor()
               .AddLogging()
               .AddPersistenceModule(configuration)
               .AddAssemblyServices()
               .AddCustomMvcServices()
               .AddFileServices()
               .AddAsyncServices()
               .AddAutoMapper(new[] { typeof(Startup).Assembly, typeof(ApplicationModule).Assembly });

            services.AddOpenApiDocument(c => c.Title = "HttPlaceholder API");
        }
    }
}
