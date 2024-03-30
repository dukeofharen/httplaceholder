using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Implementations;

namespace HttPlaceholder.Web.Shared.Utilities.Implementations;

/// <summary>
///     A class that is used to help start up HttPlaceholder.
/// </summary>
public class ProgramUtility(
    ITcpService tcpService,
    IIpService ipService) : IProgramUtility
{
    /// <summary>
    ///     Constructs a <see cref="ProgramUtility" /> instance.
    /// </summary>
    public ProgramUtility() : this(new TcpService(), new IpService())
    {
    }

    /// <inheritdoc />
    public (IEnumerable<int> httpPorts, IEnumerable<int> httpsPorts) GetPorts(SettingsModel settings)
    {
        IEnumerable<int> httpsPortsResult = Array.Empty<int>();
        var httpPortsResult = HandlePorts(settings.Web.HttpPort, DefaultConfiguration.DefaultHttpPort);
        if (settings.Web.UseHttps && !string.IsNullOrWhiteSpace(settings.Web.PfxPath) &&
            !string.IsNullOrWhiteSpace(settings.Web.PfxPassword))
        {
            httpsPortsResult = HandlePorts(settings.Web.HttpsPort, DefaultConfiguration.DefaultHttpsPort);
        }

        return (httpPortsResult, httpsPortsResult);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetHostnames()
    {
        var result = new List<string> { "127.0.0.1", "localhost" };
        var localIp = ipService.GetLocalIpAddress();
        if (!string.IsNullOrWhiteSpace(localIp))
        {
            result.Add(localIp);
        }

        return result;
    }

    private List<int> HandlePorts(string portInput, int defaultPort)
    {
        var result = new List<int>();
        var ports = ParsePorts(portInput);

        // If no specific port is configured, the default port will be used.
        // If that port happens to be taken, look for the next free TCP port.
        if (ports.Length == 1 && ports[0] == defaultPort &&
            tcpService.PortIsTaken(ports[0]))
        {
            result.Add(tcpService.GetNextFreeTcpPort());
        }
        else
        {
            EnsureNoPortsAreTaken(ports);
            result.AddRange(ports);
        }

        return result;
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

    private void EnsureNoPortsAreTaken(IEnumerable<int> ports)
    {
        var portsTaken = ports.Where(tcpService.PortIsTaken).ToArray();
        if (portsTaken.Length != 0)
        {
            throw new ArgumentException($"The following ports are already taken: {string.Join(", ", portsTaken)}");
        }
    }
}
