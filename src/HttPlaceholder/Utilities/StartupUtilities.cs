using System.IO;
using System.Threading;
using HttPlaceholder.Application;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Authorization;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.HostedServices;
using HttPlaceholder.Hubs;
using HttPlaceholder.Infrastructure;
using HttPlaceholder.Infrastructure.Web;
using HttPlaceholder.Middleware;
using HttPlaceholder.Persistence;
using HttPlaceholder.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace HttPlaceholder.Utilities;

/// <summary>
/// A class that is used to configure .NET for HttPlaceholder.
/// </summary>
public static class StartupUtilities
{
    /// <summary>
    /// Add the necessary HttPlaceholder classes to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddHttPlaceholder(this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .Configure<SettingsModel>(configuration)
            .AddInfrastructureModule()
            .AddApplicationModule()
            .AddPersistenceModule(configuration)
            .Scan(scan => scan.FromCallingAssembly().RegisterDependencies())
            .AddSignalRHubs()
            .AddHostedServices(configuration)
            .AddResourcesModule()
            .AddAutoMapper(
                config => config.AllowNullCollections = true,
                typeof(Startup).Assembly,
                typeof(ApplicationModule).Assembly);

    /// <summary>
    /// Adds a file server for serving the user interface.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="loadStaticFiles">Whether to serve the user interface or not.</param>
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
                EnableDefaultFiles = true, FileProvider = new PhysicalFileProvider(path), RequestPath = "/ph-ui"
            });
        }

        return app;
    }

    /// <summary>
    /// Adds a file server for serving several other static files
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static IApplicationBuilder UsePhStatic(this IApplicationBuilder app)
    {
        var path = $"{AssemblyHelper.GetCallingAssemblyRootPath()}/ph-static";
        return app.UseFileServer(new FileServerOptions
        {
            EnableDefaultFiles = true, FileProvider = new PhysicalFileProvider(path), RequestPath = "/ph-static"
        });
    }

    /// <summary>
    /// Preloads the stub sources.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="preloadStubs">True if the stub sources should be preloaded, false otherwise.</param>
    public static IApplicationBuilder PreloadStubs(this IApplicationBuilder app, bool preloadStubs)
    {
        if (!preloadStubs)
        {
            return app;
        }

        // Check if the stubs can be loaded.
        var stubContainer = app.ApplicationServices.GetService<IStubContext>();
        stubContainer?.PrepareAsync(CancellationToken.None).GetAwaiter().GetResult();

        return app;
    }

    /// <summary>
    /// Registers HttPlaceholder on the application builder.
    /// </summary>
    /// <param name="app">The application builder.</param>
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

    /// <summary>
    /// Adds an OpenAPI document to HttPlaceholder with custom configuration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static IApplicationBuilder UseCustomOpenApi(this IApplicationBuilder app) =>
        app.UseOpenApi(config =>
        {
            config.PostProcess = (document, _) => OpenApiUtilities.PostProcessOpenApiDocument(document);
        });
}
