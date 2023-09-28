using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;

/// <summary>
///     A command handler for deleting a scenario.
/// </summary>
public class DeleteScenarioCommandHandler : IRequestHandler<DeleteScenarioCommand, Unit>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="DeleteScenarioCommandHandler" /> instance.
    /// </summary>
    public DeleteScenarioCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _stubContext.DeleteScenarioAsync(request.ScenarioName, cancellationToken))
        {
            throw new NotFoundException($"Scenario '{request.ScenarioName}' not found.");
        }

        return Unit.Value;
    }
}
