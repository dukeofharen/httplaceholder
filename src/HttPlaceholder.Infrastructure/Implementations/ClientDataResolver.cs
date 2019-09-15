using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class ClientDataResolver : IClientDataResolver
    {
        private const string ForwardedHeaderKey = "X-Forwarded-For";
        private const string ForwardedHostKey = "X-Forwarded-Host";
        private const string ForwardedProtoKey = "X-Forwarded-Proto";

        // I've seen Nginx use this IP when reverse proxying. .NET loopback check doesn't recognize this IP as loopback IP.
        [SuppressMessage("SonarQube", "S1313", Justification = "Unrecognized loopback IP")]
        private static IPAddress NginxProxyIp { get; } = IPAddress.Parse("::ffff:127.0.0.1");
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientDataResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            var forwardedHeader = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedHeaderKey, StringComparison.OrdinalIgnoreCase) == true);
            if (forwardedHeader.Key != null && RequestIsFromLoopback(ip))
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                string forwardedFor = forwardedHeader.Value;
                var parts = forwardedFor.Split(new[] {", "}, StringSplitOptions.None);
                return parts.First();
            }

            return ip.ToString();
        }

        public string GetHost()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedHostKey, StringComparison.OrdinalIgnoreCase) == true);
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            if (header.Key != null && RequestIsFromLoopback(ip))
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                return header.Value;
            }

            return request.Host.ToString();
        }

        public bool IsHttps()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var header = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedProtoKey, StringComparison.OrdinalIgnoreCase) == true);
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            if (header.Key != null && RequestIsFromLoopback(ip))
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                return header.Value.ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
            }

            return request.IsHttps;
        }

        private bool RequestIsFromLoopback(IPAddress ip) =>
            IPAddress.IsLoopback(ip) || ip.Equals(NginxProxyIp);
    }
}
