namespace HttPlaceholder.Common;

/// <summary>
///     Describes a utility class for working with TCP connections.
/// </summary>
public interface ITcpService
{
    /// <summary>
    ///     Checks whether a given port was taken.
    /// </summary>
    /// <param name="port">The port to check.</param>
    /// <returns>True if the port was taken, false otherwise.</returns>
    bool PortIsTaken(int port);

    /// <summary>
    ///     Gets a free TCP port.
    /// </summary>
    /// <returns>The free TCP port.</returns>
    int GetNextFreeTcpPort();
}
