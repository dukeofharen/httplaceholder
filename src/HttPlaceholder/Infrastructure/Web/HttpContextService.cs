using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Infrastructure.Web
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
                var proto = _clientDataResolver.IsHttps() ? "https" : "http";
                var host = _clientDataResolver.GetHost();
                string path = _httpContextAccessor.HttpContext.Request.Path;
                var query = _httpContextAccessor.HttpContext.Request.QueryString.HasValue
                    ? _httpContextAccessor.HttpContext.Request.QueryString.Value
                    : string.Empty;
                return $"{proto}://{host}{path}{query}";
            }
        }

        public string RootUrl {
            get
            {
                var proto = _clientDataResolver.IsHttps() ? "https" : "http";
                var host = _clientDataResolver.GetHost();
                return $"{proto}://{host}";
            }
        }

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

        public byte[] GetBodyAsBytes()
        {
            var context = _httpContextAccessor.HttpContext;
            using var ms = new MemoryStream();
            context.Request.Body.CopyTo(ms);
            context.Request.Body.Position = 0;
            return ms.ToArray();
        }

        public IDictionary<string, string> GetQueryStringDictionary() =>
            _httpContextAccessor.HttpContext.Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());

        public string GetQueryString() => _httpContextAccessor.HttpContext.Request.QueryString.Value;

        public IDictionary<string, string> GetHeaders() =>
            _httpContextAccessor.HttpContext.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString());

        public TObject GetItem<TObject>(string key)
        {
            var item = _httpContextAccessor.HttpContext?.Items[key];
            return (TObject)item;
        }

        public void SetItem(string key, object item) => _httpContextAccessor.HttpContext?.Items.Add(key, item);

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
            if (httpContext.Response.Headers.ContainsKey(key))
            {
                return false;
            }

            httpContext.Response.Headers.Add(key, values);
            return true;

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

        public async Task WriteAsync(string body)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            await httpContext.Response.Body.WriteAsync(bodyBytes, 0, bodyBytes.Length);
        }
    }
}
