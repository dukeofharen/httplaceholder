using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands;

/// <summary>
///     A command for deleting a request.
/// </summary>
public class DeleteRequestCommand(string correlationId) : IRequest<bool>
{
    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; } = correlationId;
}

/// <summary>
///     A command handler for deleting a request.
/// </summary>
public class DeleteRequestCommandHandler(IStubContext stubContext) : IRequestHandler<DeleteRequestCommand, bool>
{
    /// <inheritdoc />
    public async Task<bool> Handle(DeleteRequestCommand request, CancellationToken cancellationToken) =>
        await stubContext.DeleteRequestAsync(request.CorrelationId, cancellationToken);
}
