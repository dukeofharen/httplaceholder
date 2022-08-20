using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <summary>
/// An abstract class for SignalR hubs.
/// </summary>
public abstract class BaseHub : Hub
{
    private readonly ILoginService _loginService;

    /// <summary>
    /// Constructs a <see cref="BaseHub"/> instance.
    /// </summary>
    protected BaseHub(ILoginService loginService)
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
