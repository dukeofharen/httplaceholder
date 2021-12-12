using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteAllRequest;

public class DeleteAllRequestsCommandHandler : IRequestHandler<DeleteAllRequestsCommand>
{
    private readonly IStubContext _stubContext;

    public DeleteAllRequestsCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<Unit> Handle(DeleteAllRequestsCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.DeleteAllRequestResultsAsync();
        return Unit.Value;
    }
}