﻿using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Web.Shared.Formatters;
using HttPlaceholder.Web.Shared.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            .UseSwaggerUi()
            .UseGui(settings?.Gui?.EnableUserInterface == true)
            .PreloadStubs(preloadStubs)
            .UseRouting();

    /// <summary>
    ///     Configures the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection ConfigureServices<TStartup>(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddMvc()
            .AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .AddApplicationPart(typeof(TStartup).Assembly);
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
