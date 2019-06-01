using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.UpdateStubsInTenant
{
    public class UpdateStubsInTenantCommandHandler : IRequestHandler<UpdateStubsInTenantCommand>
    {
        private readonly IStubContext _stubContext;

        public UpdateStubsInTenantCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<Unit> Handle(UpdateStubsInTenantCommand request, CancellationToken cancellationToken)
        {
            await _stubContext.UpdateAllStubs(request.Tenant, request.Stubs);
            return Unit.Value;
        }
    }
}
