using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetByStubId
{
    // ReSharper disable once UnusedType.Global
    public class GetByStubIdQueryHandler : IRequestHandler<GetByStubIdQuery, IEnumerable<RequestResultModel>>
    {
        private readonly IStubContext _stubContext;

        public GetByStubIdQueryHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<IEnumerable<RequestResultModel>> Handle(GetByStubIdQuery request, CancellationToken cancellationToken) =>
            await _stubContext.GetRequestResultsByStubIdAsync(request.StubId);
    }
}
