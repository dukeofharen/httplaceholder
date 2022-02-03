using System.Threading.Tasks;
using HttPlaceholder.Dto.v1.Requests;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class RequestNotify : IRequestNotify
{
    private readonly IHubContext<RequestHub> _hubContext;

    /// <summary>
    /// Constructs a <see cref="RequestNotify"/> instance.
    /// </summary>
    public RequestNotify(IHubContext<RequestHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc />
    public async Task NewRequestReceivedAsync(RequestOverviewDto request) => await _hubContext.Clients.All.SendAsync("RequestReceived", request);
}
