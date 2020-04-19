using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetAllStubs
{
    // ReSharper disable once UnusedType.Global
    public class GetAllStubsQueryHandler : IRequestHandler<GetAllStubsQuery, IEnumerable<FullStubModel>>
    {
        private readonly IStubContext _stubContext;

        public GetAllStubsQueryHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<IEnumerable<FullStubModel>> Handle(GetAllStubsQuery request, CancellationToken cancellationToken) =>
            await _stubContext.GetStubsAsync();
    }
}
