using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <summary>
/// The request SignalR hub.
/// </summary>
public class RequestHub : Hub
{
    private readonly ILoginService _loginService;

    /// <summary>
    /// Constructs a <see cref="RequestHub"/> instance.
    /// </summary>
    /// <param name="loginService"></param>
    public RequestHub(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <inheritdoc />
    public override Task OnConnectedAsync()
    {
        if (!_loginService.CheckLoginCookie())
        {
            throw new InvalidOperationException("NOT AUTHORIZED!");
        }

        return base.OnConnectedAsync();
    }
}
