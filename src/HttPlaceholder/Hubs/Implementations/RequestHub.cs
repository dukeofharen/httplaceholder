using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Web.Shared.Hubs;

namespace HttPlaceholder.Hubs.Implementations;

/// <summary>
///     The request SignalR hub.
/// </summary>
public class RequestHub : BaseHub
{
    /// <summary>
    ///     Constructs a <see cref="RequestHub" /> instance.
    /// </summary>
    public RequestHub(ILoginService loginService) : base(loginService)
    {
    }
}
