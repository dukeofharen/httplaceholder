using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands;

/// <summary>
///     A command for deleting all requests.
/// </summary>
public class DeleteAllRequestsCommand : IRequest<Unit>
{
}

/// <summary>
///     A command handler for deleting all requests.
/// </summary>
public class DeleteAllRequestsCommandHandler(IStubContext stubContext) : IRequestHandler<DeleteAllRequestsCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllRequestsCommand request, CancellationToken cancellationToken)
    {
        await stubContext.DeleteAllRequestResultsAsync(cancellationToken);
        return Unit.Value;
    }
}
