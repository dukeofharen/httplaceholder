using HttPlaceholder.Application;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Infrastructure;
using HttPlaceholder.Persistence;
using HttPlaceholder.Web.Shared.Middleware;
using HttPlaceholder.WebInfrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace HttPlaceholder.Web.Shared.Utilities;

/// <summary>
///     A class that is used to configure .NET for HttPlaceholder.
/// </summary>
public static class StartupUtilities
{
    /// <summary>
    ///     Add the necessary HttPlaceholder classes to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddHttPlaceholder(this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .Configure<SettingsModel>(configuration)
            .AddWebInfrastructureModule()
            .AddInfrastructureModule()
            .AddApplicationModule()
            .AddPersistenceModule(configuration)
            .Scan(scan => scan.FromCallingAssembly().RegisterDependencies())
            .AddAutoMapper(
                config => config.AllowNullCollections = true,
                typeof(StartupUtilities).Assembly,
                typeof(ApplicationModule).Assembly);

    /// <summary>
    ///     Adds a file server for serving the user interface.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="loadStaticFiles">Whether to serve the user interface or not.</param>
    public static IApplicationBuilder UseGui(this IApplicationBuilder app, bool loadStaticFiles)
    {
        if (!loadStaticFiles)
        {
            return app;
        }

        var guiPath = $"{AssemblyHelper.GetCallingAssemblyRootPath()}/gui";
        if (Directory.Exists(guiPath))
        {
            app.UseMiddleware<IndexHtmlMiddleware>(guiPath);
            app.UseFileServer(new FileServerOptions
            {
                EnableDefaultFiles = true, FileProvider = new PhysicalFileProvider(guiPath), RequestPath = "/ph-ui"
            });
        }

        return app;
    }

    /// <summary>
    ///     Adds a file server for serving several other static files
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
    ///     Preloads the stub sources.
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
    ///     Registers HttPlaceholder on the application builder.
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

            // Enable rewind here to be able to read the posted body multiple times.
            context.RequestServices.GetRequiredService<IHttpContextService>().EnableRewind();

            await next.Invoke();
        })
        .UseMiddleware<DevelopmentMiddleware>()
        .UseMiddleware<ApiExceptionHandlingMiddleware>()
        .UseMiddleware<StubHandlingMiddleware>();

    /// <summary>
    ///     Adds an OpenAPI document to HttPlaceholder with custom configuration.
    /// </summary>
    /// <param name="app">The application builder.</param>
    public static IApplicationBuilder UseCustomOpenApi(this IApplicationBuilder app) =>
        app.UseOpenApi(config =>
        {
            config.PostProcess = (document, _) => OpenApiUtilities.PostProcessOpenApiDocument(document);
        });
}
