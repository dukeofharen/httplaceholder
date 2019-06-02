using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant
{
    public class DeleteStubsInTenantCommandHandler : IRequestHandler<DeleteStubsInTenantCommand>
    {
        private readonly IStubContext _stubContext;

        public DeleteStubsInTenantCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<Unit> Handle(DeleteStubsInTenantCommand request, CancellationToken cancellationToken)
        {
            await _stubContext.DeleteAllStubsAsync(request.Tenant);
            return Unit.Value;
        }
    }
}
