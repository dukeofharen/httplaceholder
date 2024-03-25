using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries;

/// <summary>
///     A query for retrieving all stubs belonging to a tenant.
/// </summary>
public class GetStubsInTenantQuery(string tenant) : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Gets the tenant.
    /// </summary>
    public string Tenant { get; } = tenant;
}

/// <summary>
///     A query handler for retrieving all stubs belonging to a tenant.
/// </summary>
public class GetStubsInTenantQueryHandler(IStubContext stubContext) : IRequestHandler<GetStubsInTenantQuery, IEnumerable<FullStubModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(GetStubsInTenantQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetStubsAsync(request.Tenant, cancellationToken);
}
