using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries;

/// <summary>
///     A query for retrieving all stubs.
/// </summary>
public class GetAllStubsQuery : IRequest<IEnumerable<FullStubModel>>;

/// <summary>
///     A query handler for retrieving all stubs.
/// </summary>
public class GetAllStubsQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetAllStubsQuery, IEnumerable<FullStubModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>>
        Handle(GetAllStubsQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetStubsAsync(cancellationToken);
}
