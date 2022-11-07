using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Implementations;

namespace HttPlaceholder.Utilities.Implementations;

/// <summary>
///     A class that is used to help start up HttPlaceholder.
/// </summary>
public class ProgramUtility : IProgramUtility
{
    private readonly ITcpService _tcpService;

    /// <summary>
    ///     Constructs a <see cref="ProgramUtility"/> instance.
    /// </summary>
    public ProgramUtility() : this(new TcpService())
    {
    }

    /// <summary>
    ///     Constructs a <see cref="ProgramUtility"/> instance.
    /// </summary>
    private ProgramUtility(ITcpService tcpService)
    {
        _tcpService = tcpService;
    }

    /// <inheritdoc />
    public (IEnumerable<int> httpPorts, IEnumerable<int> httpsPorts) GetPorts(SettingsModel settings)
    {
        var httpPortsResult = new List<int>();
        var httpsPortsResult = new List<int>();

        // TODO combine these 2 blocks?
        var httpPorts = ParsePorts(settings.Web.HttpPort);
        if (httpPorts.Length == 1 && httpPorts[0] == DefaultConfiguration.DefaultHttpPort &&
            _tcpService.PortIsTaken(httpPorts[0]))
        {
            httpPortsResult.Add(_tcpService.GetNextFreeTcpPort());
        }
        else
        {
            httpPortsResult.AddRange(httpPorts);
        }

        if (settings.Web.UseHttps && !string.IsNullOrWhiteSpace(settings.Web.PfxPath) &&
            !string.IsNullOrWhiteSpace(settings.Web.PfxPassword))
        {
            var httpsPorts = ParsePorts(settings.Web.HttpsPort);
            if (httpsPorts.Length == 1 && httpsPorts[0] == DefaultConfiguration.DefaultHttpsPort &&
                _tcpService.PortIsTaken(httpsPorts[0]))
            {
                httpsPortsResult.Add(_tcpService.GetNextFreeTcpPort());
            }
            else
            {
                httpsPortsResult.AddRange(httpsPorts);
            }
        }

        return (httpPortsResult, httpsPortsResult);
    }

    private static int[] ParsePorts(string input)
    {
        var result = new List<int>();
        var httpPorts = input.Split(',').Select(p => p.Trim());
        foreach (var port in httpPorts)
        {
            if (!int.TryParse(port, out var parsedPort) || parsedPort is < 1 or > 65535)
            {
                throw new ArgumentException($"Port '{port}' is invalid.");
            }

            result.Add(parsedPort);
        }

        return result.ToArray();
    }
}
