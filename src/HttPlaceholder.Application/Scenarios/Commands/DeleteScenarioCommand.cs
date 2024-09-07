using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
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
    public async Task<Unit> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken) =>
        await stubContext.DeleteScenarioAsync(request.ScenarioName, cancellationToken)
            .IfAsync(r => !r,
                _ => throw new NotFoundException(string.Format(ApplicationResources.ScenarioNotFound,
                    request.ScenarioName)))
            .MapAsync(_ => Unit.Value);
}
