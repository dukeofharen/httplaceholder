using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequestsOverview;

/// <summary>
///     A query for retrieving a list of overview requests.
/// </summary>
public class GetRequestsOverviewQuery : IRequest<IEnumerable<RequestOverviewModel>>
{
    /// <summary>
    ///     Constructs a <see cref="GetRequestsOverviewQuery"/>.
    /// </summary>
    public GetRequestsOverviewQuery(string fromIdentifier, int? itemsPerPage)
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
