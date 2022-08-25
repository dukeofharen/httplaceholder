using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Infrastructure.Web;

/// <inheritdoc />
public class HttpContextService : IHttpContextService
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructs a <see cref="HttpContextService"/> instance.
    /// </summary>
    public HttpContextService(
        IClientDataResolver clientDataResolver,
        IHttpContextAccessor httpContextAccessor)
    {
        _clientDataResolver = clientDataResolver;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string Method => _httpContextAccessor.HttpContext.Request.Method;

    /// <inheritdoc />
    public string Path => _httpContextAccessor.HttpContext.Request.Path;

    /// <inheritdoc />
    public string FullPath =>
        $"{_httpContextAccessor.HttpContext.Request.Path}{_httpContextAccessor.HttpContext.Request.QueryString}";

    /// <inheritdoc />
    public string DisplayUrl
    {
        get
        {
            var proto = _clientDataResolver.IsHttps() ? "https" : "http";
            var host = _clientDataResolver.GetHost();
            string path = _httpContextAccessor.HttpContext.Request.Path;
            var query = _httpContextAccessor.HttpContext.Request.QueryString.HasValue
                ? _httpContextAccessor.HttpContext.Request.QueryString.Value
                : string.Empty;
            return $"{proto}://{host}{path}{query}";
        }
    }

    /// <inheritdoc />
    public string RootUrl
    {
        get
        {
            var proto = _clientDataResolver.IsHttps() ? "https" : "http";
            var host = _clientDataResolver.GetHost();
            return $"{proto}://{host}";
        }
    }

    /// <inheritdoc />
    public string GetBody()
    {
        var context = _httpContextAccessor.HttpContext;
        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            false,
            1024,
            true);
        var body = reader.ReadToEnd();
        context.Request.Body.Position = 0;
        return body;
    }

    /// <inheritdoc />
    public byte[] GetBodyAsBytes()
    {
        var context = _httpContextAccessor.HttpContext;
        using var ms = new MemoryStream();
        context.Request.Body.CopyTo(ms);
        context.Request.Body.Position = 0;
        return ms.ToArray();
    }

    /// <inheritdoc />
    public IDictionary<string, string> GetQueryStringDictionary() =>
        _httpContextAccessor.HttpContext.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());

    /// <inheritdoc />
    public string GetQueryString() => _httpContextAccessor.HttpContext.Request.QueryString.Value;

    /// <inheritdoc />
    public IDictionary<string, string> GetHeaders() =>
        _httpContextAccessor.HttpContext.Request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

    /// <inheritdoc />
    public TObject GetItem<TObject>(string key)
    {
        var item = _httpContextAccessor.HttpContext?.Items[key];
        return (TObject)item;
    }

    /// <inheritdoc />
    public void SetItem(string key, object item) => _httpContextAccessor.HttpContext?.Items.Add(key, item);

    /// <inheritdoc />
    public (string, StringValues)[] GetFormValues()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext.Request.Form
            .Select(f => (f.Key, f.Value))
            .ToArray();
    }

    /// <inheritdoc />
    public void SetStatusCode(int statusCode)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.Response.StatusCode = statusCode;
    }

    /// <inheritdoc />
    public void SetStatusCode(HttpStatusCode statusCode) => SetStatusCode((int)statusCode);

    /// <inheritdoc />
    public void AddHeader(string key, StringValues values)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.Response.Headers.Add(key, values);
    }

    /// <inheritdoc />
    public bool TryAddHeader(string key, StringValues values)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext.Response.Headers.ContainsKey(key))
        {
            return false;
        }

        httpContext.Response.Headers.Add(key, values);
        return true;
    }

    /// <inheritdoc />
    public void EnableRewind()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.Request.EnableBuffering();
    }

    /// <inheritdoc />
    public void ClearResponse()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.Response.Clear();
    }

    /// <inheritdoc />
    public async Task WriteAsync(byte[] body)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        await httpContext.Response.Body.WriteAsync(body, 0, body.Length);
    }

    /// <inheritdoc />
    public async Task WriteAsync(string body)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        await httpContext.Response.Body.WriteAsync(bodyBytes, 0, bodyBytes.Length);
    }

    /// <inheritdoc />
    public void SetUser(ClaimsPrincipal principal) => _httpContextAccessor.HttpContext.User = principal;

    /// <inheritdoc />
    public void AbortConnection() => _httpContextAccessor.HttpContext?.Features.Get<IConnectionLifetimeFeature>()?.Abort();
}
