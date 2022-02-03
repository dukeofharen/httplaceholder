using System.Collections.Generic;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetTenantNames;

/// <summary>
/// A query for retrieving all tenant names.
/// </summary>
public class GetTenantNamesQuery : IRequest<IEnumerable<string>>
{
}
