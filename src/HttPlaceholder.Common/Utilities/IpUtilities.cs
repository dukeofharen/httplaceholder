using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            return null;
        }

        try
        {
            var result = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                            i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(i => i.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .FirstOrDefault();
            if (result != null)
            {
                return result.Address.ToString();
            }
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
}
