using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetTenantNames;

/// <summary>
///     A query handler for retrieving all tenant names.
/// </summary>
public class GetTenantNamesQueryHandler : IRequestHandler<GetTenantNamesQuery, IEnumerable<string>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetTenantNamesQueryHandler" /> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public GetTenantNamesQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> Handle(GetTenantNamesQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetTenantNamesAsync(cancellationToken);
}
