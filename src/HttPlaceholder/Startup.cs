using Ducode.Essentials.Assembly;
using Ducode.Essentials.Async;
using Ducode.Essentials.Files;
using Ducode.Essentials.Mvc;
using HttPlaceholder.Formatters;
using HttPlaceholder.Hubs;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
            ConfigureStatic(app, true, true);

        public static void ConfigureStatic(IApplicationBuilder app, bool preloadStubs, bool loadStaticFiles) =>
            app
               .UseSignalRHubs()
               .UseHttPlaceholder()
               .UseMvc()
               .UseSwagger()
               .UseSwaggerUi3()
               .UseGui(loadStaticFiles)
               .PreloadStubs(preloadStubs);

        public static void ConfigureServicesStatic(IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddMvc(o => o.AddYamlFormatting())
               .AddJsonOptions(o => o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
               .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services
               .AddHttPlaceholder(configuration)
               .AddHttpContextAccessor()
               .AddLogging()
               .AddAssemblyServices()
               .AddCustomMvcServices()
               .AddFileServices()
               .AddAsyncServices()
               .AddOpenApiDocument(c => c.Title = "HttPlaceholder API");
        }
    }
}
