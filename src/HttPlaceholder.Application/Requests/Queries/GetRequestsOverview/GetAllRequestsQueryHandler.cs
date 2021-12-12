using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequestsOverview;

public class
    GetRequestsOverviewQueryHandler : IRequestHandler<GetRequestsOverviewQuery, IEnumerable<RequestOverviewModel>>
{
    private readonly IStubContext _stubContext;

    public GetRequestsOverviewQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<IEnumerable<RequestOverviewModel>> Handle(GetRequestsOverviewQuery request,
        CancellationToken cancellationToken) =>
        await _stubContext.GetRequestResultsOverviewAsync();
}