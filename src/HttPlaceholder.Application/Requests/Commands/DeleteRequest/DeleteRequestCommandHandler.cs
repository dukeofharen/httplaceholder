using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest
{
    public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand, bool>
    {
        private readonly IStubContext _stubContext;

        public DeleteRequestCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<bool> Handle(DeleteRequestCommand request, CancellationToken cancellationToken) =>
            await _stubContext.DeleteRequestAsync(request.CorrelationId);
    }
}
