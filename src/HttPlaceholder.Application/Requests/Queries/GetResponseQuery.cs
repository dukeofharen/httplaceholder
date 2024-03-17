using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries;

/// <summary>
///     A query for retrieving a response.
/// </summary>
public class GetResponseQuery(string correlationId) : IRequest<ResponseModel>
{
    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; } = correlationId;
}

/// <summary>
///     A query handler for retrieving a response.
/// </summary>
public class GetResponseQueryHandler(IStubContext stubContext) : IRequestHandler<GetResponseQuery, ResponseModel>
{
    /// <inheritdoc />
    public async Task<ResponseModel> Handle(GetResponseQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetResponseAsync(request.CorrelationId, cancellationToken)
            .IfNull(() => throw new NotFoundException("response", request.CorrelationId));
}
