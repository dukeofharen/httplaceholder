using System;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Infrastructure.Web;

internal class UrlResolver(
    IClientDataResolver clientDataResolver,
    IHttpContextAccessor httpContextAccessor,
    IOptionsMonitor<SettingsModel> options)
    : IUrlResolver, ISingletonService
{
    public string GetDisplayUrl()
    {
        var rootUrl = GetRootUrl();
        var httpContext = httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("HttpContext not set.");
        string path = httpContext.Request.Path;
        var query = httpContext.Request.QueryString.HasValue
            ? httpContext.Request.QueryString.Value
            : string.Empty;
        return $"{rootUrl}{path}{query}";
    }

    public string GetRootUrl()
    {
        var settings = options.CurrentValue;
        if (!string.IsNullOrWhiteSpace(settings?.Web?.PublicUrl))
        {
            return settings.Web.PublicUrl.EnsureDoesntEndWith('/');
        }

        var proto = clientDataResolver.IsHttps() ? "https" : "http";
        var host = clientDataResolver.GetHost();
        return $"{proto}://{host}";
    }
}
