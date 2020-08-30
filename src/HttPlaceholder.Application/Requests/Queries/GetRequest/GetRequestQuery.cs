using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequest
{
    public class GetRequestQuery : IRequest<RequestResultModel>
    {
        public string CorrelationId { get; set; }
    }
}
