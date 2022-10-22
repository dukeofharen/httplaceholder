using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStubsOverview;

/// <summary>
///     A query for retrieving a stub overview.
/// </summary>
public class GetStubsOverviewQuery : IRequest<IEnumerable<FullStubOverviewModel>>
{
}
