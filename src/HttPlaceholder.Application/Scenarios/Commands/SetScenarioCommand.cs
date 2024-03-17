using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands;

/// <summary>
///     A command for setting a scenario.
/// </summary>
public class SetScenarioCommand(ScenarioStateModel scenarioStateModel, string scenarioName) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the scenario.
    /// </summary>
    public ScenarioStateModel ScenarioStateModel { get; } = scenarioStateModel;

    /// <summary>
    ///     Gets the scenario name.
    /// </summary>
    public string ScenarioName { get; } = scenarioName;
}

/// <summary>
///     A command handler for setting a scenario.
/// </summary>
public class SetScenarioCommandHandler(IStubContext stubContext) : IRequestHandler<SetScenarioCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(SetScenarioCommand request, CancellationToken cancellationToken)
    {
        await stubContext.SetScenarioAsync(request.ScenarioName, request.ScenarioStateModel, cancellationToken);
        return Unit.Value;
    }
}
