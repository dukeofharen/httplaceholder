using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStubsOverview;

/// <summary>
/// A query handler for retrieving a stub overview.
/// </summary>
public class
    GetStubsOverviewQueryHandler : IRequestHandler<GetStubsOverviewQuery, IEnumerable<FullStubOverviewModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetStubsOverviewQueryHandler"/> instance.
    /// </summary>
    public GetStubsOverviewQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubOverviewModel>> Handle(
        GetStubsOverviewQuery request,
        CancellationToken cancellationToken) =>
        await _stubContext.GetStubsOverviewAsync();
}
