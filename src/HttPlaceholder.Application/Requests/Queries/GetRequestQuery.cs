using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries;

/// <summary>
///     A query for retrieving a request.
/// </summary>
public class GetRequestQuery(string correlationId) : IRequest<RequestResultModel>
{
    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; } = correlationId;
}

/// <summary>
///     A query handler for retrieving a request.
/// </summary>
public class GetRequestQueryHandler(IStubContext stubContext) : IRequestHandler<GetRequestQuery, RequestResultModel>
{
    /// <inheritdoc />
    public async Task<RequestResultModel> Handle(GetRequestQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetRequestResultAsync(request.CorrelationId, cancellationToken)
            .IfNullAsync(() => throw new NotFoundException("request", request.CorrelationId));
}
