using System;
using System.Linq;
using System.Net;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Web;

/// <inheritdoc />
public class ClientDataResolver : IClientDataResolver
{
    private const string ForwardedHeaderKey = "X-Forwarded-For";
    private const string ForwardedHostKey = "X-Forwarded-Host";
    private const string ForwardedProtoKey = "X-Forwarded-Proto";

    // I've seen Nginx use this IP when reverse proxying. .NET loopback check doesn't recognize this IP as loopback IP.
    private static IPAddress NginxProxyIp { get; } = IPAddress.Parse("::ffff:127.0.0.1");
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructs a <see cref="ClientDataResolver"/> instance.
    /// </summary>
    public ClientDataResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string GetClientIp()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
        var (key, value) = request.Headers.FirstOrDefault(h =>
            h.Key.Equals(ForwardedHeaderKey, StringComparison.OrdinalIgnoreCase));
        if (key == null || !RequestIsFromLoopback(ip))
        {
            return ip.ToString();
        }

        // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
        string forwardedFor = value;
        var parts = forwardedFor.Split(new[] { ", " }, StringSplitOptions.None);
        return parts.First();
    }

    /// <inheritdoc />
    public string GetHost()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var (key, value) = request.Headers.FirstOrDefault(h =>
            h.Key?.Equals(ForwardedHostKey, StringComparison.OrdinalIgnoreCase) == true);
        var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
        if (key != null && RequestIsFromLoopback(ip))
        {
            // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
            return value;
        }

        return request.Host.ToString();
    }

    /// <inheritdoc />
    public bool IsHttps()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        var (key, value) = request.Headers.FirstOrDefault(h =>
            h.Key?.Equals(ForwardedProtoKey, StringComparison.OrdinalIgnoreCase) == true);
        var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
        if (key != null && RequestIsFromLoopback(ip))
        {
            // TODO in a later stage, check the reverse proxy against a list of "safe" proxy IPs.
            return value.ToString().Equals("https", StringComparison.OrdinalIgnoreCase);
        }

        return request.IsHttps;
    }

    private static bool RequestIsFromLoopback(IPAddress ip) =>
        IPAddress.IsLoopback(ip) || ip.Equals(NginxProxyIp);
}
