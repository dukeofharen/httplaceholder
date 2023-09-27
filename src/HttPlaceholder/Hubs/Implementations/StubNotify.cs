using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class StubNotify : IStubNotify
{
    private readonly IHubContext<StubHub> _hubContext;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Constructs a <see cref="StubNotify" /> instance.
    /// </summary>
    public StubNotify(IHubContext<StubHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task StubAddedAsync(FullStubOverviewModel stub, string distributionKey = null, CancellationToken cancellationToken = default)
    {
        var input = _mapper.Map<FullStubOverviewDto>(stub);
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);
        await channel.SendAsync("StubAdded", input, cancellationToken);
    }

    /// <inheritdoc />
    public async Task StubDeletedAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);
        await channel.SendAsync("StubDeleted", stubId, cancellationToken);
    }
}
