using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetAllRequests
{
    // ReSharper disable once UnusedType.Global
    public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, IEnumerable<RequestResultModel>>
    {
        private readonly IStubContext _stubContext;

        public GetAllRequestsQueryHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<IEnumerable<RequestResultModel>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken) =>
            await _stubContext.GetRequestResultsAsync();
    }
}
