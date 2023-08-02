using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Web.Shared.Hubs;

/// <summary>
///     An abstract class for SignalR hubs.
/// </summary>
public abstract class BaseHub : Hub
{
    private readonly ILoginCookieService _loginCookieService;

    /// <summary>
    ///     Constructs a <see cref="BaseHub" /> instance.
    /// </summary>
    protected BaseHub(ILoginCookieService loginCookieService)
    {
        _loginCookieService = loginCookieService;
    }

    /// <inheritdoc />
    public override Task OnConnectedAsync()
    {
        if (!_loginCookieService.CheckLoginCookie())
        {
            throw new InvalidOperationException("NOT AUTHORIZED!");
        }

        return base.OnConnectedAsync();
    }
}
