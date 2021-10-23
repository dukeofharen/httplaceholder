using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HttPlaceholder.Common.Utilities
{
    public class TcpUtilities
    {
        public static bool PortIsTaken(int port) =>
            IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Any(l => l.Port == port);

        public static int GetNextFreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
