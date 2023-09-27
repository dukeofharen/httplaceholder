using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Web.Shared.Dto.v1.Scenarios;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class ScenarioNotify : IScenarioNotify
{
    private readonly IHubContext<ScenarioHub> _hubContext;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Constructs a <see cref="ScenarioNotify" /> instance.
    /// </summary>
    public ScenarioNotify(IHubContext<ScenarioHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task ScenarioSetAsync(ScenarioStateModel scenario, string distributionKey = null, CancellationToken cancellationToken = default)
    {
        var input = _mapper.Map<ScenarioStateDto>(scenario);
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);
        await channel.SendAsync("ScenarioSet", input, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ScenarioDeletedAsync(string scenarioName, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);
        await channel.SendAsync("ScenarioDeleted", scenarioName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AllScenariosDeletedAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var channel = string.IsNullOrWhiteSpace(distributionKey)
            ? _hubContext.Clients.All
            : _hubContext.Clients.Group(distributionKey);
        await channel.SendAsync("AllScenariosDeleted", cancellationToken);
    }
}
