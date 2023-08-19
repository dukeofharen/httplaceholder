using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class UrlResolver : IUrlResolver, ISingletonService
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public UrlResolver(
        IClientDataResolver clientDataResolver,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<SettingsModel> options)
    {
        _clientDataResolver = clientDataResolver;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    public string GetDisplayUrl()
    {
        var rootUrl = GetRootUrl();
        string path = _httpContextAccessor.HttpContext.Request.Path;
        var query = _httpContextAccessor.HttpContext.Request.QueryString.HasValue
            ? _httpContextAccessor.HttpContext.Request.QueryString.Value
            : string.Empty;
        return $"{rootUrl}{path}{query}";
    }

    public string GetRootUrl()
    {
        var settings = _options.CurrentValue;
        if (!string.IsNullOrWhiteSpace(settings?.Web?.PublicUrl))
        {
            return settings.Web.PublicUrl.EnsureDoesntEndWith('/');
        }

        var proto = _clientDataResolver.IsHttps() ? "https" : "http";
        var host = _clientDataResolver.GetHost();
        return $"{proto}://{host}";
    }
}
