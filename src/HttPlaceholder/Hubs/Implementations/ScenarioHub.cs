using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Web.Shared.Hubs;

namespace HttPlaceholder.Hubs.Implementations;

/// <summary>
///     The request SignalR hub.
/// </summary>
public class ScenarioHub : BaseHub
{
    /// <summary>
    ///     Constructs a <see cref="ScenarioHub" /> instance.
    /// </summary>
    public ScenarioHub(ILoginService loginService) : base(loginService)
    {
    }
}
