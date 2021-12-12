using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub;

public class DeleteStubCommandHandler : IRequestHandler<DeleteStubCommand>
{
    private readonly IStubContext _stubContext;

    public DeleteStubCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<Unit> Handle(DeleteStubCommand request, CancellationToken cancellationToken)
    {
        if (!await _stubContext.DeleteStubAsync(request.StubId))
        {
            throw new NotFoundException(nameof(StubModel), request.StubId);
        }

        return Unit.Value;
    }
}