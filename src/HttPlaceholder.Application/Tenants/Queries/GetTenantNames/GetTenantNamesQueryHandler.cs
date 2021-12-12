using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetTenantNames;

public class GetTenantNamesQueryHandler : IRequestHandler<GetTenantNamesQuery, IEnumerable<string>>
{
    private readonly IStubContext _stubContext;

    public GetTenantNamesQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<IEnumerable<string>> Handle(GetTenantNamesQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetTenantNamesAsync();
}