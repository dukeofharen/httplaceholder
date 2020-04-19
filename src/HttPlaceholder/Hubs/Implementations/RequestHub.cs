using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations
{
    /// <summary>
    /// The request SignalR hub.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RequestHub : Hub
    {
        private readonly ILoginService _loginService;

        public RequestHub(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public override Task OnConnectedAsync()
        {
            if (!_loginService.CheckLoginCookie())
            {
                throw new InvalidOperationException("NOT AUTHORIZED!");
            }

            return base.OnConnectedAsync();
        }
    }
}
