using System.Net;
using System.Security.Claims;
using System.Text;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class HttpContextService : IHttpContextService, ISingletonService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpContextService> _logger;

    /// <summary>
    ///     Constructs a <see cref="HttpContextService" /> instance.
    /// </summary>
    public HttpContextService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public string Method => GetContext().Request.Method;

    /// <inheritdoc />
    public string Path => GetContext().Request.Path;

    /// <inheritdoc />
    public string FullPath
    {
        get
        {
            var context = GetContext();
            return $"{context.Request.Path}{context.Request.QueryString}";
        }
    }

    /// <inheritdoc />
    public async Task<string> GetBodyAsync(CancellationToken cancellationToken) =>
        Encoding.UTF8.GetString(await GetBodyAsBytesAsync(cancellationToken));

    /// <inheritdoc />
    public async Task<byte[]> GetBodyAsBytesAsync(CancellationToken cancellationToken)
    {
        var context = GetContext();
        using var ms = new MemoryStream();
        await context.Request.Body.CopyToAsync(ms, cancellationToken);
        context.Request.Body.Position = 0;
        return ms.ToArray();
    }

    /// <inheritdoc />
    public IDictionary<string, string> GetQueryStringDictionary() =>
        GetContext().Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());

    /// <inheritdoc />
    public string GetQueryString() => GetContext().Request.QueryString.Value;

    /// <inheritdoc />
    public IDictionary<string, string> GetHeaders() =>
        GetContext().Request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

    /// <inheritdoc />
    public TObject GetItem<TObject>(string key)
    {
        var item = _httpContextAccessor.HttpContext?.Items[key];
        return (TObject)item;
    }

    /// <inheritdoc />
    public void SetItem(string key, object item)
    {
        var context = GetContext();
        if (context.Items.ContainsKey(key))
        {
            context.Items.Remove(key);
        }

        context.Items.Add(key, item);
    }

    /// <inheritdoc />
    public bool DeleteItem(string key)
    {
        var context = GetContext();
        return context.Items.ContainsKey(key) && context.Items.Remove(key);
    }

    /// <inheritdoc />
    public async Task<(string, StringValues)[]> GetFormValuesAsync(CancellationToken cancellationToken = default)
    {
        var context = GetContext();
        if (!context.Request.HasFormContentType)
        {
            return Array.Empty<(string, StringValues)>();
        }

        try
        {
            var form = await context.Request.ReadFormAsync(cancellationToken);
            return form
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
    public void SetStatusCode(int statusCode) => GetContext().Response.StatusCode = statusCode;

    /// <inheritdoc />
    public void SetStatusCode(HttpStatusCode statusCode) => SetStatusCode((int)statusCode);

    /// <inheritdoc />
    public void AddHeader(string key, StringValues values) => GetContext().Response.Headers.Append(key, values);

    /// <inheritdoc />
    public bool TryAddHeader(string key, StringValues values)
    {
        var httpContext = GetContext();
        if (httpContext.Response.Headers.ContainsKey(key))
        {
            return false;
        }

        httpContext.Response.Headers.Append(key, values);
        return true;
    }

    /// <inheritdoc />
    public void EnableRewind() => GetContext().Request.EnableBuffering();

    /// <inheritdoc />
    public void ClearResponse() => GetContext().Response.Clear();

    /// <inheritdoc />
    public async Task WriteAsync(byte[] body, CancellationToken cancellationToken) =>
        await GetContext().Response.Body.WriteAsync(body, cancellationToken);

    /// <inheritdoc />
    public async Task WriteAsync(string body, CancellationToken cancellationToken)
    {
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        await GetContext().Response.Body.WriteAsync(bodyBytes, cancellationToken);
    }

    /// <inheritdoc />
    public void SetUser(ClaimsPrincipal principal) => GetContext().User = principal;

    /// <inheritdoc />
    public void AbortConnection() => GetContext().Abort();

    /// <inheritdoc />
    public void AppendCookie(string key, string value, CookieOptions options) =>
        GetContext().Response.Cookies.Append(key, value, options);

    /// <inheritdoc />
    public KeyValuePair<string, string>? GetRequestCookie(string key)
    {
        var result = GetContext().Request.Cookies.FirstOrDefault(c => c.Key == key);
        if (string.IsNullOrWhiteSpace(result.Key))
        {
            return null;
        }

        return result;
    }

    private HttpContext GetContext() =>
        ThrowHelper.ThrowIfNull<HttpContext, InvalidOperationException>(_httpContextAccessor.HttpContext);
}
