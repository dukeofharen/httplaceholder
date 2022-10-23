using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Infrastructure.Web;

internal class HttpContextService : IHttpContextService, ISingletonService
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ILogger<HttpContextService> _logger;

    /// <summary>
    ///     Constructs a <see cref="HttpContextService" /> instance.
    /// </summary>
    public HttpContextService(
        IClientDataResolver clientDataResolver,
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpContextService> logger)
    {
        _clientDataResolver = clientDataResolver;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
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
    public async Task<string> GetBodyAsync(CancellationToken cancellationToken) =>
        Encoding.UTF8.GetString(await GetBodyAsBytesAsync(cancellationToken));

    /// <inheritdoc />
    public async Task<byte[]> GetBodyAsBytesAsync(CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        using var ms = new MemoryStream();
        await context.Request.Body.CopyToAsync(ms, cancellationToken);
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
        var contentType = GetHeaders().CaseInsensitiveSearch(HeaderKeys.ContentType);
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Array.Empty<(string, StringValues)>();
        }

        if (!MimeTypes.FormMimeTypes.Any(ct => contentType.Contains(ct, StringComparison.OrdinalIgnoreCase)))
        {
            return Array.Empty<(string, StringValues)>();
        }

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.Request.Form
                .Select(f => (f.Key, f.Value))
                .ToArray();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Exception thrown while reading form data.");
            return Array.Empty<(string, StringValues)>();
        }
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
    public async Task WriteAsync(byte[] body, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        await httpContext.Response.Body.WriteAsync(body, 0, body.Length, cancellationToken);
    }

    /// <inheritdoc />
    public async Task WriteAsync(string body, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        await httpContext.Response.Body.WriteAsync(bodyBytes, 0, bodyBytes.Length, cancellationToken);
    }

    /// <inheritdoc />
    public void SetUser(ClaimsPrincipal principal) => _httpContextAccessor.HttpContext.User = principal;

    /// <inheritdoc />
    public void AbortConnection() => _httpContextAccessor.HttpContext.Abort();
}
