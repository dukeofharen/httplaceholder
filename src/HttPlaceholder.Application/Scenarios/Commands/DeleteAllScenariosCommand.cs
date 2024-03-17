using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands;

/// <summary>
///     A command for deleting all scenarios.
/// </summary>
public class DeleteAllScenariosCommand : IRequest<Unit>
{
}

/// <summary>
///     A command handler for deleting all scenarios.
/// </summary>
public class DeleteAllScenariosCommandHandler(IStubContext stubContext)
    : IRequestHandler<DeleteAllScenariosCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllScenariosCommand request, CancellationToken cancellationToken)
    {
        await stubContext.DeleteAllScenariosAsync(cancellationToken);
        return Unit.Value;
    }
}
