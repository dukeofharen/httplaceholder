using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HttPlaceholder.Services.Implementations
{
    internal class HttpContextService : IHttpContextService
    {
        private const string ForwardedHeaderKey = "X-Forwarded-For";
        private const string ForwardedHostKey = "X-Forwarded-Host";
        private const string ForwardedProtoKey = "X-Forwarded-Proto";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Method => _httpContextAccessor.HttpContext.Request.Method;

        public string Path => _httpContextAccessor.HttpContext.Request.Path;

        public string FullPath => $"{_httpContextAccessor.HttpContext.Request.Path}{_httpContextAccessor.HttpContext.Request.QueryString}";

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

        public string GetClientIp()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h => h.Key?.Equals(ForwardedHeaderKey, StringComparison.OrdinalIgnoreCase) == true);
            if (header.Key != null)
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                string forwardedFor = header.Value;
                var parts = forwardedFor.Split(new[] { ", " }, StringSplitOptions.None);
                return parts.First();
            }
            else
            {
                return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }

        public string GetHost()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h => h.Key?.Equals(ForwardedHostKey, StringComparison.OrdinalIgnoreCase) == true);
            if (header.Key != null)
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                return header.Value;
            }
            else
            {
                return request.Host.ToString();
            }
        }

        public bool IsHttps()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h => h.Key?.Equals(ForwardedProtoKey, StringComparison.OrdinalIgnoreCase) == true);
            if (header.Key != null)
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                return header.Value.ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return request.IsHttps;
            }
        }
    }
}