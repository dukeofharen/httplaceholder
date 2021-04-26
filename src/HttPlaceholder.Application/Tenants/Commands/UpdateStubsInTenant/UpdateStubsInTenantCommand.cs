using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.UpdateStubsInTenant
{
    public class UpdateStubsInTenantCommand : IRequest
    {
        public UpdateStubsInTenantCommand(IEnumerable<StubModel> stubs, string tenant)
        {
            Stubs = stubs;
            Tenant = tenant;
        }

        public IEnumerable<StubModel> Stubs { get; }
        public string Tenant { get; }
    }
}
