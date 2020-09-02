using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequestsOverview
{
    public class GetRequestsOverviewQuery : IRequest<IEnumerable<RequestOverviewModel>>
    {
    }
}
