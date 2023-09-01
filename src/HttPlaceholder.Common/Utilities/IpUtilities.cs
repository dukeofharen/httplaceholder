using System;
using System.Net;
using System.Net.Sockets;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A static utility class for working with IP addresses.
/// </summary>
public static class IpUtilities
{
    /// <summary>
    ///     This method returns the local IP address of the current machine, or null if it could not be found.
    ///     Source: https://stackoverflow.com/questions/6803073/get-local-ip-address
    /// </summary>
    /// <returns>The local IP address, or null if it was not found.</returns>
    public static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return null;
    }
}
