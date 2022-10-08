using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub;

/// <summary>
/// A command handler for deleting a stub.
/// </summary>
public class DeleteStubCommandHandler : IRequestHandler<DeleteStubCommand>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="DeleteStubCommandHandler"/> instance.
    /// </summary>
    public DeleteStubCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteStubCommand request, CancellationToken cancellationToken)
    {
        if (!await _stubContext.DeleteStubAsync(request.StubId, cancellationToken))
        {
            throw new NotFoundException(nameof(StubModel), request.StubId);
        }

        return Unit.Value;
    }
}
