using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries;

/// <summary>
///     A query for retrieving all requests.
/// </summary>
public class GetAllRequestsQuery(string fromIdentifier, int? itemsPerPage) : IRequest<IEnumerable<RequestResultModel>>
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
///     A query handler for retrieving all requests.
/// </summary>
public class GetAllRequestsQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetAllRequestsQuery, IEnumerable<RequestResultModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> Handle(
        GetAllRequestsQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetRequestResultsAsync(PagingModel.Create(request.FromIdentifier, request.ItemsPerPage),
            cancellationToken);
}
