using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class RequestNotify : IRequestNotify
{
    private readonly IHubContext<RequestHub> _hubContext;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Constructs a <see cref="RequestNotify" /> instance.
    /// </summary>
    public RequestNotify(IHubContext<RequestHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task NewRequestReceivedAsync(RequestResultModel request, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var input = _mapper.Map<RequestOverviewDto>(request);
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);

        await channel.SendAsync("RequestReceived", input, cancellationToken);
    }
}
