using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest
{
    public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand>
    {
        private readonly IStubContext _stubContext;

        public DeleteRequestCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public Task<Unit> Handle(DeleteRequestCommand request, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}
