using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries;

/// <summary>
///     A query for retrieving all tenant names.
/// </summary>
public class GetTenantNamesQuery : IRequest<IEnumerable<string>>;

/// <summary>
///     A query handler for retrieving all tenant names.
/// </summary>
public class GetTenantNamesQueryHandler(IStubContext stubContext) : IRequestHandler<GetTenantNamesQuery, IEnumerable<string>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<string>> Handle(GetTenantNamesQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetTenantNamesAsync(cancellationToken);
}
