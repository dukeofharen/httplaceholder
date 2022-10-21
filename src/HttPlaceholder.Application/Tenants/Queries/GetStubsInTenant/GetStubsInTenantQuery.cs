using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant;

/// <summary>
///     A query for retrieving all stubs belonging to a tenant.
/// </summary>
public class GetStubsInTenantQuery : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Constructs a <see cref="GetStubsInTenantQuery" /> instance.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    public GetStubsInTenantQuery(string tenant)
    {
        Tenant = tenant;
    }

    /// <summary>
    ///     Gets the tenant.
    /// </summary>
    public string Tenant { get; }
}
