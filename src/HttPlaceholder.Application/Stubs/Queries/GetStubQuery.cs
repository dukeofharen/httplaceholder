using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries;

/// <summary>
///     A query for retrieving a specific stub.
/// </summary>
public class GetStubQuery(string stubId) : IRequest<FullStubModel>
{
    /// <summary>
    ///     Gets the stub ID.
    /// </summary>
    public string StubId { get; } = stubId;
}

/// <summary>
///     A query handler for retrieving a specific stub.
/// </summary>
public class GetStubQueryHandler(IStubContext stubContext) : IRequestHandler<GetStubQuery, FullStubModel>
{
    /// <inheritdoc />
    public async Task<FullStubModel> Handle(GetStubQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetStubAsync(request.StubId, cancellationToken)
            .IfNull(() => throw new NotFoundException(nameof(StubModel), request.StubId));
}
