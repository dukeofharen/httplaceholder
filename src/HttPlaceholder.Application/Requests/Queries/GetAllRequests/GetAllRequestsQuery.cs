using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetAllRequests;

/// <summary>
///     A query for retrieving all requests.
/// </summary>
public class GetAllRequestsQuery : IRequest<IEnumerable<RequestResultModel>>
{
    /// <summary>
    ///     Constructs a <see cref="GetAllRequestsQuery" />.
    /// </summary>
    public GetAllRequestsQuery(string fromIdentifier, int? itemsPerPage)
    {
        FromIdentifier = fromIdentifier;
        ItemsPerPage = itemsPerPage;
    }

    /// <summary>
    ///     The identifier from which to find items. If this is not set; means to query from the start.
    /// </summary>
    public string FromIdentifier { get; set; }

    /// <summary>
    ///     The number of items to show on a page.
    /// </summary>
    public int? ItemsPerPage { get; }
}
