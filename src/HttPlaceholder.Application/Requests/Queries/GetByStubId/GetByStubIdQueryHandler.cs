using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetByStubId;

/// <summary>
/// A query handler for retrieving all requests by stub ID.
/// </summary>
public class GetByStubIdQueryHandler : IRequestHandler<GetByStubIdQuery, IEnumerable<RequestResultModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetByStubIdQueryHandler"/> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public GetByStubIdQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> Handle(GetByStubIdQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetRequestResultsByStubIdAsync(request.StubId, cancellationToken);
}
