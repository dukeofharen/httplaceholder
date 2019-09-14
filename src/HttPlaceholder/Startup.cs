using System.Diagnostics.CodeAnalysis;
using HttPlaceholder.Configuration;
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

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) =>
            ConfigureServicesStatic(services, Configuration);

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
            ConfigureStatic(app, true, Configuration?.Get<SettingsModel>()?.Gui?.EnableUserInterface == true);

        public static void ConfigureStatic(IApplicationBuilder app, bool preloadStubs, bool loadStaticFiles) =>
            app
                .UseSignalRHubs()
                .UseHttPlaceholder()
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUi3()
                .UseGui(loadStaticFiles)
                .PreloadStubs(preloadStubs);

        [SuppressMessage("SonarQube", "S4792")]
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
               .AddOpenApiDocument(c => c.Title = "HttPlaceholder API");
        }
    }
}
