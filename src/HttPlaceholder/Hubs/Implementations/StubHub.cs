using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Web.Shared.Hubs;

namespace HttPlaceholder.Hubs.Implementations;

/// <summary>
///     The stub SignalR hub.
/// </summary>
public class StubHub : BaseHub
{
    /// <summary>
    ///     Constructs a <see cref="StubHub" /> instance.
    /// </summary>
    public StubHub(ILoginCookieService loginCookieService) : base(loginCookieService)
    {
    }
}
