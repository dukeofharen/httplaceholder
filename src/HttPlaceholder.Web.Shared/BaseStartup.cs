using System.Reflection;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Web.Shared.Formatters;
using HttPlaceholder.Web.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HttPlaceholder.Web.Shared;

/// <summary>
///     A static class that is used to handle the startup of the application.
/// </summary>
public static class BaseStartup
{
    /// <summary>
    ///     Configures the web application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="preloadStubs">Whether to preload the stubs or not.</param>
    /// <param name="settings">The HttPlaceholder settings.</param>
    public static IApplicationBuilder Configure(this IApplicationBuilder app, bool preloadStubs,
        SettingsModel settings) =>
        app
            .UseHttPlaceholder()
            .UseCustomOpenApi()
            .UseSwaggerUi3()
            .UseGui(settings?.Gui?.EnableUserInterface == true)
            .UsePhStatic()
            .PreloadStubs(preloadStubs)
            .UseRouting();

    /// <summary>
    ///     Configures the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection ConfigureServices<TStartup>(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMvc()
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .AddApplicationPart(Assembly.GetExecutingAssembly());
        services.Configure<MvcOptions>(o =>
        {
            o.RespectBrowserAcceptHeader = true;
            o.ReturnHttpNotAcceptable = true;
            o
                .AddYamlFormatting()
                .AddPlainTextFormatting();
        });
        services
            .AddHttPlaceholder(configuration)
            .AddHttpContextAccessor()
            .AddLogging()
            .AddOpenApiDocument(c => c.Title = "HttPlaceholder API");
        return services;
    }
}
