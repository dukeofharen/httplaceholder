using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStubsOverview;

public class
    GetStubsOverviewQueryHandler : IRequestHandler<GetStubsOverviewQuery, IEnumerable<FullStubOverviewModel>>
{
    private readonly IStubContext _stubContext;

    public GetStubsOverviewQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    public async Task<IEnumerable<FullStubOverviewModel>> Handle(
        GetStubsOverviewQuery request,
        CancellationToken cancellationToken) =>
        await _stubContext.GetStubsOverviewAsync();
}