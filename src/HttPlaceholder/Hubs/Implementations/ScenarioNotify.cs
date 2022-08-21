using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Dto.v1.Scenarios;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Hubs.Implementations;

/// <inheritdoc />
public class ScenarioNotify : IScenarioNotify
{
    private readonly IHubContext<ScenarioHub> _hubContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructs a <see cref="ScenarioNotify"/> instance.
    /// </summary>
    public ScenarioNotify(IHubContext<ScenarioHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task ScenarioSetAsync(ScenarioStateModel scenario)
    {
        var input = _mapper.Map<ScenarioStateDto>(scenario);
        await _hubContext.Clients.All.SendAsync("ScenarioSet", input);
    }

    /// <inheritdoc />
    public async Task ScenarioDeletedAsync(string scenarioName) =>
        await _hubContext.Clients.All.SendAsync("ScenarioDeleted", scenarioName);

    /// <inheritdoc />
    public async Task AllScenariosDeletedAsync() => await _hubContext.Clients.All.SendAsync("AllScenariosDeleted");
}
