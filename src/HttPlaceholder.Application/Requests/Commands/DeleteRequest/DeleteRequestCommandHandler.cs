using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest
{
    public class DeleteRequestCommandHandler : IRequestHandler<DeleteRequestCommand>
    {
        public Task<Unit> Handle(DeleteRequestCommand request, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}
