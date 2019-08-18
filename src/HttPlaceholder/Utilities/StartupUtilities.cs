using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Ducode.Essentials.Assembly;
using HttPlaceholder.Application;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Authorization;
using HttPlaceholder.Configuration;
using HttPlaceholder.Hubs;
using HttPlaceholder.Infrastructure;
using HttPlaceholder.Middleware;
using HttPlaceholder.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace HttPlaceholder.Utilities
{
    public static class StartupUtilities
    {
        public static IServiceCollection AddHttPlaceholder(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<SettingsModel>(configuration)
                .AddInfrastructureModule()
                .AddApplicationModule()
                .AddPersistenceModule(configuration)
                .AddAuthorizationModule()
                .AddSignalRHubs()
                .AddAutoMapper(new[] { typeof(Startup).Assembly, typeof(ApplicationModule).Assembly });

        public static IApplicationBuilder UseGui(this IApplicationBuilder app, bool loadStaticFiles)
        {
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

            return app;
        }

        public static IApplicationBuilder PreloadStubs(this IApplicationBuilder app, bool preloadStubs)
        {
            if (preloadStubs)
            {
                // Check if the stubs can be loaded.
                var stubContainer = app.ApplicationServices.GetService<IStubContext>();
                Task.Run(() => stubContainer.PrepareAsync()).GetAwaiter().GetResult();
            }

            return app;
        }

        public static IApplicationBuilder UseHttPlaceholder(this IApplicationBuilder app) => app
            .UseMiddleware<ApiHeadersMiddleware>()
            .UseMiddleware<ApiExceptionHandlingMiddleware>()
            .UseMiddleware<StubHandlingMiddleware>();

    }
}
