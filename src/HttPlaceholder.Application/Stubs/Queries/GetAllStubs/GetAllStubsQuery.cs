using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetAllStubs;

/// <summary>
///     A query for retrieving all stubs.
/// </summary>
public class GetAllStubsQuery : IRequest<IEnumerable<FullStubModel>>
{
}
