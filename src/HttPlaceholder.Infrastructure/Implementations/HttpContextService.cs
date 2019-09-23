using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class HttpContextService : IHttpContextService
    {
        private readonly IClientDataResolver _clientDataResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(
            IClientDataResolver clientDataResolver,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientDataResolver = clientDataResolver;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Method => _httpContextAccessor.HttpContext.Request.Method;

        public string Path => _httpContextAccessor.HttpContext.Request.Path;

        public string FullPath =>
            $"{_httpContextAccessor.HttpContext.Request.Path}{_httpContextAccessor.HttpContext.Request.QueryString}";

        public string DisplayUrl
        {
            get
            {
                string proto = _clientDataResolver.IsHttps() ? "https" : "http";
                string host = _clientDataResolver.GetHost();
                string path = _httpContextAccessor.HttpContext.Request.Path;
                string query = _httpContextAccessor.HttpContext.Request.QueryString.HasValue
                    ? _httpContextAccessor.HttpContext.Request.QueryString.Value
                    : string.Empty;
                return $"{proto}://{host}{path}{query}";
            }
        }

        public string GetBody()
        {
            using (var bodyStream = new MemoryStream())
            using (var reader = new StreamReader(bodyStream))
            {
                // TODO fix this: we should make this method async and use CopyToAsync instead.
                var syncIOFeature = _httpContextAccessor.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }

                // TODO fix multiple reading of body: https://devblogs.microsoft.com/aspnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
                _httpContextAccessor.HttpContext.Request.Body.CopyTo(bodyStream);
                bodyStream.Seek(0, SeekOrigin.Begin);
                var body = reader.ReadToEnd();
                return body;
            }
        }

        public IDictionary<string, string> GetQueryStringDictionary()
        {
            return _httpContextAccessor.HttpContext.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());
        }

        public IDictionary<string, string> GetHeaders()
        {
            return _httpContextAccessor.HttpContext.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString());
        }

        public TObject GetItem<TObject>(string key)
        {
            var item = _httpContextAccessor.HttpContext?.Items[key];
            return (TObject)item;
        }

        public void SetItem(string key, object item)
        {
            _httpContextAccessor.HttpContext?.Items.Add(key, item);
        }

        public (string, StringValues)[] GetFormValues()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.Request.Form
                .Select(f => (f.Key, f.Value))
                .ToArray();
        }

        public void SetStatusCode(int statusCode)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Response.StatusCode = statusCode;
        }

        public void AddHeader(string key, StringValues values)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Response.Headers.Add(key, values);
        }

        public bool TryAddHeader(string key, StringValues values)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!httpContext.Response.Headers.ContainsKey(key))
            {
                httpContext.Response.Headers.Add(key, values);
                return true;
            }

            return false;
        }

        public void EnableRewind()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Request.EnableBuffering();
        }

        public void ClearResponse()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Response.Clear();
        }

        public async Task WriteAsync(byte[] body)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            await httpContext.Response.Body.WriteAsync(body, 0, body.Length);
        }
    }
}
