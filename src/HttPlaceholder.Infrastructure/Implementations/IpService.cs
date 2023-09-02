using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <summary>
///     A service for working with IP addresses.
/// </summary>
public class IpService : IIpService, ISingletonService
{
    /// <inheritdoc />
    public string GetLocalIpAddress() => IpUtilities.GetLocalIpAddress();
}
