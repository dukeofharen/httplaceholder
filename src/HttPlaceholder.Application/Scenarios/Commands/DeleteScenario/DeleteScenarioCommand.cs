using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;

public class DeleteScenarioCommand : IRequest
{
    public DeleteScenarioCommand(string scenarioName)
    {
        ScenarioName = scenarioName;
    }

    public string ScenarioName { get; }
}