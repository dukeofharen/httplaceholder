using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetAllRequests
{
    public class GetAllRequestsQuery : IRequest<IEnumerable<RequestResultModel>>
    {
    }
}
