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
    public async Task StubAddedAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var input = _mapper.Map<StubDto>(stub);
        await _hubContext.Clients.All.SendAsync("StubAdded", input, cancellationToken);
    }

    /// <inheritdoc />
    public async Task StubDeletedAsync(string stubId, CancellationToken cancellationToken) =>
        await _hubContext.Clients.All.SendAsync("StubUpdated", stubId, cancellationToken);

    /// <inheritdoc />
    public async Task StubUpdatedAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var input = _mapper.Map<StubDto>(stub);
        await _hubContext.Clients.All.SendAsync("StubUpdated", input, cancellationToken);
    }
}
