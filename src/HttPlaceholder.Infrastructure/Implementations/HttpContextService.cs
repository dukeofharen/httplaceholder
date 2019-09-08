using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class HttpContextService : IHttpContextService
    {
        private const string ForwardedHostKey = "X-Forwarded-Host";
        private const string ForwardedProtoKey = "X-Forwarded-Proto";
        private readonly IClientIpResolver _clientIpResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(
            IClientIpResolver clientIpResolver,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientIpResolver = clientIpResolver;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Method => _httpContextAccessor.HttpContext.Request.Method;

        public string Path => _httpContextAccessor.HttpContext.Request.Path;

        public string FullPath =>
            $"{_httpContextAccessor.HttpContext.Request.Path}{_httpContextAccessor.HttpContext.Request.QueryString}";

        public string DisplayUrl => _httpContextAccessor.HttpContext.Request.GetDisplayUrl();

        public string GetBody()
        {
            using (var bodyStream = new MemoryStream())
            using (var reader = new StreamReader(bodyStream))
            {
                _httpContextAccessor.HttpContext.Request.Body.CopyTo(bodyStream);
                _httpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
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

        public string GetHost()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedHostKey, StringComparison.OrdinalIgnoreCase) == true);
            string actualIp = _clientIpResolver.GetClientIp();
            if (header.Key != null && actualIp == "127.0.0.1")
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                // TODO any loopback IP should be able to bypass this check, not only 127.0.0.1.
                return header.Value;
            }

            return request.Host.ToString();
        }

        public bool IsHttps()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedProtoKey, StringComparison.OrdinalIgnoreCase) == true);
            string actualIp = _clientIpResolver.GetClientIp();
            if (header.Key != null && actualIp == "127.0.0.1")
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                // TODO any loopback IP should be able to bypass this check, not only 127.0.0.1.
                return header.Value.ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
            }

            return request.IsHttps;
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
            httpContext.Request.EnableRewind();
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
