using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands;

/// <summary>
///     A command for deleting a stub.
/// </summary>
public class DeleteStubCommand(string stubId) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the stub ID.
    /// </summary>
    public string StubId { get; } = stubId;
}

/// <summary>
///     A command handler for deleting a stub.
/// </summary>
public class DeleteStubCommandHandler(IStubContext stubContext) : IRequestHandler<DeleteStubCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteStubCommand request, CancellationToken cancellationToken) =>
        await stubContext.DeleteStubAsync(request.StubId, cancellationToken)
            .IfAsync(r => !r, _ => throw new NotFoundException(nameof(StubModel), request.StubId))
            .MapAsync(_ => Unit.Value);
}
