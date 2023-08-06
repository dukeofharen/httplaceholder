using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteAllRequests;

/// <summary>
///     A command handler for deleting all requests.
/// </summary>
public class DeleteAllRequestsCommandHandler : IRequestHandler<DeleteAllRequestsCommand, Unit>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="DeleteAllRequestsCommandHandler" /> instance.
    /// </summary>
    public DeleteAllRequestsCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllRequestsCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.DeleteAllRequestResultsAsync(cancellationToken);
        return Unit.Value;
    }
}
