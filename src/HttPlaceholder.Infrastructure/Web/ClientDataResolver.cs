using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Infrastructure.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NetTools;

namespace HttPlaceholder.Infrastructure.Web;

internal class ClientDataResolver : IClientDataResolver, ISingletonService
{
    private const string ForwardedHeaderKey = "X-Forwarded-For";
    private const string ForwardedHostKey = "X-Forwarded-Host";
    private const string ForwardedProtoKey = "X-Forwarded-Proto";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ClientDataResolver> _logger;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly List<IPAddress> _parsedProxyIps = [];
    private bool _parsedProxyIpsInitialized;

    /// <summary>
    ///     Constructs a <see cref="ClientDataResolver" /> instance.
    /// </summary>
    public ClientDataResolver(
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<SettingsModel> options,
        ILogger<ClientDataResolver> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _options = options;
    }

    // I've seen Nginx use this IP when reverse proxying. .NET loopback check doesn't recognize this IP as loopback IP.
    private static IPAddress NginxProxyIp { get; } = IPAddress.Parse("::ffff:127.0.0.1");

    /// <inheritdoc />
    public string GetClientIp() =>
        ParseProxyHeader(
            ForwardedHeaderKey,
            httpContext => httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty, parts => parts.First());

    /// <inheritdoc />
    public string GetHost() => ParseProxyHeader(
        ForwardedHostKey,
        httpContext => httpContext.Request.Host.ToString(),
        parts => parts.First());

    /// <inheritdoc />
    public bool IsHttps() =>
        ParseProxyHeader(
            ForwardedProtoKey,
            httpContext => httpContext.Request.IsHttps,
            parts => string.Equals(parts.First(), "https", StringComparison.OrdinalIgnoreCase));

    private T ParseProxyHeader<T>(
        string headerKey,
        Func<HttpContext, T> getDefaultValueFunc,
        Func<string[], T> parseResultFunc)
    {
        var httpContext = _httpContextAccessor.GetHttpContext();
        var settings = _options.CurrentValue;
        if (settings.Web?.ReadProxyHeaders == false)
        {
            return getDefaultValueFunc(httpContext);
        }

        var ip = httpContext.Connection.RemoteIpAddress;
        var proxyValue = httpContext.Request.Headers.CaseInsensitiveSearch(headerKey);
        if (proxyValue == default(StringValues) || !IpIsAllowed(ip))
        {
            return getDefaultValueFunc(httpContext);
        }

        string headerValue = proxyValue;
        var parts = headerValue.Split(',', StringSplitOptions.TrimEntries);
        return parseResultFunc(parts);
    }

    private bool IpIsAllowed(IPAddress ip)
    {
        if (_parsedProxyIpsInitialized)
        {
            return IpIsAllowedInternal(ip);
        }

        var settings = _options.CurrentValue;
        var safeProxyIps = settings.Web?.SafeProxyIps?.Split(",", StringSplitOptions.TrimEntries) ??
                           Array.Empty<string>();
        foreach (var proxyIp in safeProxyIps)
        {
            if (proxyIp.Contains('/'))
            {
                // Input is probably a CIDR, so parse it to a range.
                if (IPAddressRange.TryParse(proxyIp, out var range))
                {
                    _parsedProxyIps.AddRange(range);
                }
            }
            else if (IPAddress.TryParse(proxyIp, out var parsedIp))
            {
                _parsedProxyIps.Add(parsedIp);
            }
            else
            {
                _logger.LogWarning($"Could not parse IP '{proxyIp}'.");
            }
        }

        _parsedProxyIpsInitialized = true;

        return IpIsAllowedInternal(ip);

        bool IpIsAllowedInternal(IPAddress ipAddress)
        {
            return IPAddress.IsLoopback(ipAddress) ||
                   ip.Equals(NginxProxyIp) ||
                   _parsedProxyIps.Contains(ipAddress);
        }
    }
}
