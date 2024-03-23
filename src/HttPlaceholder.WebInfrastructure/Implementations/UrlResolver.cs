using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class UrlResolver(
    IClientDataResolver clientDataResolver,
    IHttpContextAccessor httpContextAccessor,
    IOptionsMonitor<SettingsModel> options)
    : IUrlResolver, ISingletonService
{
    public string GetDisplayUrl()
    {
        var rootUrl = GetRootUrl();
        string path = httpContextAccessor.HttpContext.Request.Path;
        var query = httpContextAccessor.HttpContext.Request.QueryString.HasValue
            ? httpContextAccessor.HttpContext.Request.QueryString.Value
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
