using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteAllStubs;

public class DeleteAllStubsCommandHandler : IRequestHandler<DeleteAllStubsCommand>
{
    private readonly IStubContext _stubContext;

    public DeleteAllStubsCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<Unit> Handle(DeleteAllStubsCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.DeleteAllStubsAsync();
        return Unit.Value;
    }
}