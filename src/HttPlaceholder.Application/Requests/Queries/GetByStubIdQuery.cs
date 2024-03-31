using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries;

/// <summary>
///     A query for retrieving all requests by stub ID.
/// </summary>
public class GetByStubIdQuery(string stubId) : IRequest<IEnumerable<RequestResultModel>>
{
    /// <summary>
    ///     Gets the stub ID.
    /// </summary>
    public string StubId { get; } = stubId;
}

/// <summary>
///     A query handler for retrieving all requests by stub ID.
/// </summary>
public class GetByStubIdQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetByStubIdQuery, IEnumerable<RequestResultModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> Handle(GetByStubIdQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetRequestResultsByStubIdAsync(request.StubId, cancellationToken);
}
