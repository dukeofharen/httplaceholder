using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Options;
using NetTools;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class HostnameValidator(IOptionsMonitor<SettingsModel> options) : IHostnameValidator, ISingletonService
{
    /// <inheritdoc/>
    public bool HostnameIsValid(string hostname)
    {
        hostname = hostname.Trim().ToLower();
        var settings = options.CurrentValue;
        var enableReverseProxy = settings?.Stub?.EnableReverseProxy ?? false;
        var allowedHosts = FormatHosts(settings?.Stub?.AllowedHosts);
        var disallowedHosts = FormatHosts(settings?.Stub?.DisallowedHosts);
        if (!enableReverseProxy && allowedHosts.Length == 0 && disallowedHosts.Length == 0)
        {
            // Proxy is not enabled and there are no hosts configured, so return "false".
            return false;
        }

        if (allowedHosts.Length == 0 && disallowedHosts.Length == 0)
        {
            // Proxy is enabled and no hosts are configured, so return "true".
            return true;
        }

        if (allowedHosts.Length > 0)
        {
            return HostIsValid(allowedHosts, hostname);
        }

        return !HostIsValid(disallowedHosts, hostname);
    }

    private static string[] FormatHosts(string input) => string.IsNullOrWhiteSpace(input)
        ? Array.Empty<string>()
        : input.Split(",", StringSplitOptions.TrimEntries).Select(e => e.Trim().ToLower()).ToArray();

    private static bool HostIsValid(string[] hostsToCheck, string host)
    {
        // Host can both be an IP address or actual hostname.
        // The "hosts" array can contain hostnames, IP addresses and IP ranges.
        if (IPAddress.TryParse(host, out var parsedIp))
        {
            // Input is IP address, so check if IP is whitelisted based on the input.
            foreach (var hostToCheck in hostsToCheck)
            {
                if (IPAddressRange.TryParse(hostToCheck, out var range))
                {
                    if (range.Contains(parsedIp))
                    {
                        return true;
                    }
                }
            }
        }

        // Input is hostname. First check on whole hostname and then, if there is still no match, if there is a regex match.
        foreach (var hostToCheck in hostsToCheck)
        {
            if (hostToCheck.Equals(host, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                if (Regex.IsMatch(host, hostToCheck))
                {
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }
        }

        return false;
    }
}
