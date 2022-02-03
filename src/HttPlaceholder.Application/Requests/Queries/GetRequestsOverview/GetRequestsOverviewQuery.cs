using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequestsOverview;

/// <summary>
/// A query for retrieving a list of overview requests.
/// </summary>
public class GetRequestsOverviewQuery : IRequest<IEnumerable<RequestOverviewModel>>
{
}
