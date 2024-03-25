using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <summary>
///     A utility class for working with TCP connections.
/// </summary>
public class TcpService : ITcpService, ISingletonService
{
    /// <inheritdoc />
    public bool PortIsTaken(int port) => IPGlobalProperties
        .GetIPGlobalProperties()
        .GetActiveTcpListeners()
        .Any(l => l.Port == port);

    /// <inheritdoc />
    public int GetNextFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
