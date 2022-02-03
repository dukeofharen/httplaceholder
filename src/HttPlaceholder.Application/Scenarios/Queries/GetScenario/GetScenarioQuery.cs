using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetScenario;

/// <summary>
/// A query for retrieving a scenario.
/// </summary>
public class GetScenarioQuery : IRequest<ScenarioStateModel>
{
    /// <summary>
    /// Constructs a <see cref="GetScenarioQuery"/> instance.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    public GetScenarioQuery(string scenario)
    {
        Scenario = scenario;
    }

    /// <summary>
    /// Gets the scenario name.
    /// </summary>
    public string Scenario { get; }
}
