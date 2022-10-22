using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetAllRequests;

/// <summary>
///     A query for retrieving all requests.
/// </summary>
public class GetAllRequestsQuery : IRequest<IEnumerable<RequestResultModel>>
{
}
