using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <summary>
///     A utility class for working with TCP connections.
/// </summary>
public class TcpService : ITcpService, ISingletonService
{
    /// <inheritdoc />
    public bool PortIsTaken(int port) => TcpUtilities.PortIsTaken(port);

    /// <inheritdoc />
    public int GetNextFreeTcpPort() => TcpUtilities.GetNextFreeTcpPort();
}
