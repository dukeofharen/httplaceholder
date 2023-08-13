using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class UrlResolver : IUrlResolver, ISingletonService
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlResolver(
        IClientDataResolver clientDataResolver,
        IHttpContextAccessor httpContextAccessor)
    {
        _clientDataResolver = clientDataResolver;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetDisplayUrl()
    {
        var proto = _clientDataResolver.IsHttps() ? "https" : "http";
        var host = _clientDataResolver.GetHost();
        string path = _httpContextAccessor.HttpContext.Request.Path;
        var query = _httpContextAccessor.HttpContext.Request.QueryString.HasValue
            ? _httpContextAccessor.HttpContext.Request.QueryString.Value
            : string.Empty;
        return $"{proto}://{host}{path}{query}";
    }

    public string GetRootUrl()
    {
        var proto = _clientDataResolver.IsHttps() ? "https" : "http";
        var host = _clientDataResolver.GetHost();
        return $"{proto}://{host}";
    }
}
