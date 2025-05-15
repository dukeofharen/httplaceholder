using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries;

/// <summary>
///     A query for retrieving a stub overview.
/// </summary>
public class GetStubsOverviewQuery : IRequest<IEnumerable<FullStubOverviewModel>>;

/// <summary>
///     A query handler for retrieving a stub overview.
/// </summary>
public class
    GetStubsOverviewQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetStubsOverviewQuery, IEnumerable<FullStubOverviewModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubOverviewModel>> Handle(
        GetStubsOverviewQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetStubsOverviewAsync(cancellationToken);
}
