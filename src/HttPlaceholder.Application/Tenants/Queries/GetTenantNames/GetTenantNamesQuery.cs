using System.Collections.Generic;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Queries.GetTenantNames;

public class GetTenantNamesQuery : IRequest<IEnumerable<string>>
{
}