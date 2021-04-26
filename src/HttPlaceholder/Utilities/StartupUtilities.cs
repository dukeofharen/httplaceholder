using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Authorization;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Hubs;
using HttPlaceholder.Infrastructure;
using HttPlaceholder.Infrastructure.Web;
using HttPlaceholder.Middleware;
using HttPlaceholder.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace HttPlaceholder.Utilities
{
    public static class StartupUtilities
    {
        public static IServiceCollection AddHttPlaceholder(this IServiceCollection services,
            IConfiguration configuration) =>
            services
                .Configure<SettingsModel>(configuration)
                .AddInfrastructureModule()
                .AddApplicationModule()
                .AddPersistenceModule(configuration)
                .AddAuthorizationModule()
                .AddSignalRHubs()
                .AddWebInfrastructure()
                .AddAutoMapper(
                    config => config.AllowNullCollections = true,
                    typeof(Startup).Assembly,
                    typeof(ApplicationModule).Assembly);

        private static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientDataResolver, ClientDataResolver>();
            services.TryAddSingleton<IHttpContextService, HttpContextService>();
            return services;
        }

        public static IApplicationBuilder UseGui(this IApplicationBuilder app, bool loadStaticFiles)
        {
            if (!loadStaticFiles)
            {
                return app;
            }

            var path = $"{AssemblyHelper.GetCallingAssemblyRootPath()}/gui";
            if (Directory.Exists(path))
            {
                app.UseFileServer(new FileServerOptions
                {
                    EnableDefaultFiles = true,
                    FileProvider = new PhysicalFileProvider(path),
                    RequestPath = "/ph-ui"
                });
            }

            return app;
        }

        public static IApplicationBuilder UsePhStatic(this IApplicationBuilder app)
        {
            var path = $"{AssemblyHelper.GetCallingAssemblyRootPath()}/ph-static";
            return app.UseFileServer(new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileProvider = new PhysicalFileProvider(path),
                RequestPath = "/ph-static"
            });
        }

        public static IApplicationBuilder PreloadStubs(this IApplicationBuilder app, bool preloadStubs)
        {
            if (!preloadStubs)
            {
                return app;
            }

            // Check if the stubs can be loaded.
            var stubContainer = app.ApplicationServices.GetService<IStubContext>();
            Task.Run(() => stubContainer.PrepareAsync()).GetAwaiter().GetResult();

            return app;
        }

        public static IApplicationBuilder UseHttPlaceholder(this IApplicationBuilder app) => app
            .Use(async (context, next) =>
            {
                // TODO fix this: the body should always be retrieved asynchronously.
                var syncIoFeature = context.Features.Get<IHttpBodyControlFeature>();
                if (syncIoFeature != null)
                {
                    syncIoFeature.AllowSynchronousIO = true;
                }

                await next.Invoke();
            })
            .UseMiddleware<ApiHeadersMiddleware>()
            .UseMiddleware<ApiExceptionHandlingMiddleware>()
            .UseMiddleware<StubHandlingMiddleware>();
    }
}
