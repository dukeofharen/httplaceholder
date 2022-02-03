using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;

/// <summary>
/// A command for deleting a scenario.
/// </summary>
public class DeleteScenarioCommand : IRequest
{
    /// <summary>
    /// Constructs a <see cref="DeleteScenarioCommand"/> instance.
    /// </summary>
    /// <param name="scenarioName">The scenario name.</param>
    public DeleteScenarioCommand(string scenarioName)
    {
        ScenarioName = scenarioName;
    }

    /// <summary>
    /// Gets the scenario name.
    /// </summary>
    public string ScenarioName { get; }
}
