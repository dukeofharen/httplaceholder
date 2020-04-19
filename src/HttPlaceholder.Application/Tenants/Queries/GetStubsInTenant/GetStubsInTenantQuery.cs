using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant
{
    public class GetStubsInTenantQuery : IRequest<IEnumerable<FullStubModel>>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Tenant { get; set; }
    }
}
