using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant;

public class GetStubsInTenantQueryHandler : IRequestHandler<GetStubsInTenantQuery, IEnumerable<FullStubModel>>
{
    private readonly IStubContext _stubContext;

    public GetStubsInTenantQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<IEnumerable<FullStubModel>> Handle(GetStubsInTenantQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetStubsAsync(request.Tenant);
}