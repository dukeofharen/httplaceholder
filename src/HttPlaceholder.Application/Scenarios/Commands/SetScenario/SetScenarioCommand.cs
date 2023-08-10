using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.SetScenario;

/// <summary>
///     A command for setting a scenario.
/// </summary>
public class SetScenarioCommand : IRequest<Unit>
{
    /// <summary>
    ///     Constructs a <see cref="SetScenarioCommand" /> instance
    /// </summary>
    /// <param name="scenarioStateModel">The scenario.</param>
    /// <param name="scenarioName">The scenario name.</param>
    public SetScenarioCommand(ScenarioStateModel scenarioStateModel, string scenarioName)
    {
        ScenarioStateModel = scenarioStateModel;
        ScenarioName = scenarioName;
    }

    /// <summary>
    ///     Gets the scenario.
    /// </summary>
    public ScenarioStateModel ScenarioStateModel { get; }

    /// <summary>
    ///     Gets the scenario name.
    /// </summary>
    public string ScenarioName { get; }
}
