using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetScenario;

public class GetScenarioQuery : IRequest<ScenarioStateModel>
{
    public GetScenarioQuery(string scenario)
    {
        Scenario = scenario;
    }

    public string Scenario { get; }
}