using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetByStubId;

/// <summary>
/// A query for retrieving all requests by stub ID.
/// </summary>
public class GetByStubIdQuery : IRequest<IEnumerable<RequestResultModel>>
{
    /// <summary>
    /// Constructs a <see cref="GetByStubIdQuery"/> instance.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    public GetByStubIdQuery(string stubId)
    {
        StubId = stubId;
    }

    /// <summary>
    /// Gets the stub ID.
    /// </summary>
    public string StubId { get; }
}
