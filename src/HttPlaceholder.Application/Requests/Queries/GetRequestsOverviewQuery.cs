using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries;

/// <summary>
///     A query for retrieving a list of overview requests.
/// </summary>
public class GetRequestsOverviewQuery(string fromIdentifier, int? itemsPerPage)
    : IRequest<IEnumerable<RequestOverviewModel>>
{
    /// <summary>
    ///     The identifier from which to find items. If this is not set; means to query from the start.
    /// </summary>
    public string FromIdentifier { get; } = fromIdentifier;

    /// <summary>
    ///     The number of items to show on a page.
    /// </summary>
    public int? ItemsPerPage { get; } = itemsPerPage;
}

/// <summary>
///     A query handler for retrieving a list of overview requests.
/// </summary>
public class GetRequestsOverviewQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetRequestsOverviewQuery, IEnumerable<RequestOverviewModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> Handle(GetRequestsOverviewQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetRequestResultsOverviewAsync(
            PagingModel.Create(request.FromIdentifier, request.ItemsPerPage), cancellationToken);
}
