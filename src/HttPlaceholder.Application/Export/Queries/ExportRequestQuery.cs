using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Export.Queries;

/// <summary>
///     A query for exporting a request.
/// </summary>
public class ExportRequestQuery : IRequest<string>
{
    /// <summary>
    ///     Constructs a <see cref="ExportRequestQuery" /> instance.
    /// </summary>
    public ExportRequestQuery(string correlationId, RequestExportType requestExportType)
    {
        CorrelationId = correlationId;
        RequestExportType = requestExportType;
    }

    /// <summary>
    ///     Gets the correlation ID.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    ///     Gets the request export type.
    /// </summary>
    public RequestExportType RequestExportType { get; }
}

/// <summary>
///     A query handler for exporting a request.
/// </summary>
public class ExportRequestQueryHandler(
    IStubContext stubContext,
    IRequestToCurlCommandService requestToCurlCommandService,
    IRequestToHarService requestToHarService) : IRequestHandler<ExportRequestQuery, string>
{
    /// <inheritdoc />
    public async Task<string> Handle(ExportRequestQuery request, CancellationToken cancellationToken)
    {
        var requestResult = await stubContext.GetRequestResultAsync(request.CorrelationId, cancellationToken)
            .IfNullAsync(() => throw new NotFoundException("request", request.CorrelationId));
        var response = await stubContext.GetResponseAsync(request.CorrelationId, cancellationToken);
        switch (request.RequestExportType)
        {
            case RequestExportType.Curl:
                return requestToCurlCommandService.Convert(requestResult);
            case RequestExportType.Har:
                NotFoundException.ThrowIfNull(response, "response", request.CorrelationId);
                return requestToHarService.Convert(requestResult, response);
            default:
                throw new NotImplementedException(
                    $"Converting of request to {request.RequestExportType} not supported.");
        }
    }
}
