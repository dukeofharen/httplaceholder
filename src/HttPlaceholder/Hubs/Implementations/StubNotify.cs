using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using HttPlaceholder.Web.Shared.Utilities;
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
    public async Task StubAddedAsync(FullStubOverviewModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var input = _mapper.Map<FullStubOverviewDto>(stub);
        await _hubContext.GetChannel(distributionKey).SendAsync("StubAdded", input, cancellationToken);
    }

    /// <inheritdoc />
    public async Task StubDeletedAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        await _hubContext.GetChannel(distributionKey).SendAsync("StubDeleted", stubId, cancellationToken);

    /// <inheritdoc />
    public async Task ReloadStubsAsync(string distributionKey = null, CancellationToken cancellationToken = default) =>
        await _hubContext.GetChannel(distributionKey).SendAsync("ReloadStubs", cancellationToken);
}
