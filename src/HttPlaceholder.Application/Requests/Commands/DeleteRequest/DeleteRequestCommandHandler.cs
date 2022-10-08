using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest;

/// <summary>
/// A command handler for deleting a request.
/// </summary>
public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="DeleteRequestCommandHandler"/> instance.
    /// </summary>
    public DeleteRequestCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<bool> Handle(DeleteRequestCommand request, CancellationToken cancellationToken) =>
        await _stubContext.DeleteRequestAsync(request.CorrelationId, cancellationToken);
}
