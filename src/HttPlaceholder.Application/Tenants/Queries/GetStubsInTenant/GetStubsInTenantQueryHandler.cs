using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant;

/// <summary>
/// A query handler for retrieving all stubs belonging to a tenant.
/// </summary>
public class GetStubsInTenantQueryHandler : IRequestHandler<GetStubsInTenantQuery, IEnumerable<FullStubModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetStubsInTenantQueryHandler"/> instance.
    /// </summary>
    public GetStubsInTenantQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(GetStubsInTenantQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetStubsAsync(request.Tenant);
}
