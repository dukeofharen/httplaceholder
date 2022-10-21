using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with TCP connections.
/// </summary>
public class TcpUtilities
{
    /// <summary>
    ///     Checks whether a given port was taken.
    /// </summary>
    /// <param name="port">The port to check.</param>
    /// <returns>True if the port was taken, false otherwise.</returns>
    public static bool PortIsTaken(int port) =>
        IPGlobalProperties
            .GetIPGlobalProperties()
            .GetActiveTcpListeners()
            .Any(l => l.Port == port);

    /// <summary>
    ///     Gets a free TCP port.
    /// </summary>
    /// <returns>The free TCP port.</returns>
    public static int GetNextFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
