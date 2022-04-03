using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetResponse;

/// <summary>
/// A query for retrieving a response.
/// </summary>
public class GetResponseQuery : IRequest<ResponseModel>
{
    /// <summary>
    /// Constructs a <see cref="GetResponseQuery"/> instance.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    public GetResponseQuery(string correlationId)
    {
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; }
}
