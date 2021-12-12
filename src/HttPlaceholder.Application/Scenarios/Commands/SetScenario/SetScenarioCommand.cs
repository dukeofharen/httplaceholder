using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.SetScenario;

public class SetScenarioCommand : IRequest
{
    public SetScenarioCommand(ScenarioStateModel scenarioStateModel, string scenarioName)
    {
        ScenarioStateModel = scenarioStateModel;
        ScenarioName = scenarioName;
    }

    public ScenarioStateModel ScenarioStateModel { get; }

    public string ScenarioName { get; }
}