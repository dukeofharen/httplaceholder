using System;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class ClientIpResolver : IClientIpResolver
    {
        private const string ForwardedHeaderKey = "X-Forwarded-For";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientIpResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string actualIp =
                IpUtilities.NormalizeIp(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
            var forwardedHeader = request.Headers.FirstOrDefault(h =>
                h.Key?.Equals(ForwardedHeaderKey, StringComparison.OrdinalIgnoreCase) == true);
            if (forwardedHeader.Key != null && actualIp == "127.0.0.1")
            {
                // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
                string forwardedFor = forwardedHeader.Value;
                var parts = forwardedFor.Split(new[] {", "}, StringSplitOptions.None);
                string forwardedIp = IpUtilities.NormalizeIp(parts.First());
                return forwardedIp;
            }

            return actualIp;
        }
    }
}
