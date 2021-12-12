using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetScenario;

public class GetScenarioQueryHandler : IRequestHandler<GetScenarioQuery, ScenarioStateModel>
{
    private readonly IScenarioService _scenarioService;

    public GetScenarioQueryHandler(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    public Task<ScenarioStateModel> Handle(GetScenarioQuery request, CancellationToken cancellationToken)
    {
        var scenario = _scenarioService.GetScenario(request.Scenario);
        if (scenario == null)
        {
            throw new NotFoundException($"Scenario with name '{request.Scenario}'.");
        }

        return Task.FromResult(scenario);
    }
}