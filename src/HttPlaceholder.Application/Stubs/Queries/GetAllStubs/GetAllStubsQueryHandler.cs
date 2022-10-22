using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetAllStubs;

/// <summary>
///     A query handler for retrieving all stubs.
/// </summary>
public class GetAllStubsQueryHandler : IRequestHandler<GetAllStubsQuery, IEnumerable<FullStubModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetAllStubsQueryHandler" /> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public GetAllStubsQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>>
        Handle(GetAllStubsQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetStubsAsync(cancellationToken);
}
