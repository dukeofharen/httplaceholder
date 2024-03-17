using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands;

/// <summary>
///     A command for deleting a scenario.
/// </summary>
public class DeleteScenarioCommand(string scenarioName) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the scenario name.
    /// </summary>
    public string ScenarioName { get; } = scenarioName;
}

/// <summary>
///     A command handler for deleting a scenario.
/// </summary>
public class DeleteScenarioCommandHandler(IStubContext stubContext) : IRequestHandler<DeleteScenarioCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken)
    {
        // TODO
        if (!await stubContext.DeleteScenarioAsync(request.ScenarioName, cancellationToken))
        {
            throw new NotFoundException($"Scenario '{request.ScenarioName}' not found.");
        }

        // TODO
        return Unit.Value;
    }
}
