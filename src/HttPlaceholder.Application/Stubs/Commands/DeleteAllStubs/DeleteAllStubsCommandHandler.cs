using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteAllStubs;

/// <summary>
///     A command handler for deleting all stubs.
/// </summary>
public class DeleteAllStubsCommandHandler : IRequestHandler<DeleteAllStubsCommand>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="DeleteAllStubsCommandHandler" /> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public DeleteAllStubsCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteAllStubsCommand request, CancellationToken cancellationToken) =>
        await _stubContext.DeleteAllStubsAsync(cancellationToken);
}
