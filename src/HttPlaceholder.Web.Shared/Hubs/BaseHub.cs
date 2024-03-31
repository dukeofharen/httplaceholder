using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Web.Shared.Hubs;

/// <summary>
///     An abstract class for SignalR hubs.
/// </summary>
public abstract class BaseHub(ILoginCookieService loginCookieService) : Hub
{
    /// <inheritdoc />
    public override Task OnConnectedAsync()
    {
        if (!loginCookieService.CheckLoginCookie())
        {
            throw new InvalidOperationException("NOT AUTHORIZED!");
        }

        return base.OnConnectedAsync();
    }
}
