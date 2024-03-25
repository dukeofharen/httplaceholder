using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands;

/// <summary>
///     A command for deleting all stubs.
/// </summary>
public class DeleteAllStubsCommand : IRequest<Unit>;

/// <summary>
///     A command handler for deleting all stubs.
/// </summary>
public class DeleteAllStubsCommandHandler(IStubContext stubContext) : IRequestHandler<DeleteAllStubsCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllStubsCommand request, CancellationToken cancellationToken)
    {
        await stubContext.DeleteAllStubsAsync(cancellationToken);
        return Unit.Value;
    }
}
