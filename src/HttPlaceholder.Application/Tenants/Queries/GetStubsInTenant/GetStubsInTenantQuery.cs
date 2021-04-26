using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant
{
    public class GetStubsInTenantQuery : IRequest<IEnumerable<FullStubModel>>
    {
        public GetStubsInTenantQuery(string tenant)
        {
            Tenant = tenant;
        }

        public string Tenant { get; }
    }
}
