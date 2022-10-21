using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequest;

/// <summary>
///     A query for retrieving a request.
/// </summary>
public class GetRequestQuery : IRequest<RequestResultModel>
{
    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; set; }
}
