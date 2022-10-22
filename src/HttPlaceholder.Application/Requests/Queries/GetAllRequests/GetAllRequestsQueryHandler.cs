using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetAllRequests;

/// <summary>
///     A query handler for retrieving all requests.
/// </summary>
public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, IEnumerable<RequestResultModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetAllRequestsQueryHandler" /> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public GetAllRequestsQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> Handle(GetAllRequestsQuery request,
        CancellationToken cancellationToken) =>
        await _stubContext.GetRequestResultsAsync(cancellationToken);
}
