﻿using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Export.Queries.ExportRequest;

/// <summary>
///     A query handler for exporting a request.
/// </summary>
public class ExportRequestQueryHandler : IRequestHandler<ExportRequestQuery, string>
{
    private readonly IStubContext _stubContext;
    private readonly IRequestToCurlCommandService _requestToCurlCommandService;
    private readonly IRequestToHarService _requestToHarService;

    /// <summary>
    ///     Constructs an <see cref="ExportRequestQueryHandler"/> instance.
    /// </summary>
    public ExportRequestQueryHandler(IStubContext stubContext, IRequestToCurlCommandService requestToCurlCommandService,
        IRequestToHarService requestToHarService)
    {
        _stubContext = stubContext;
        _requestToCurlCommandService = requestToCurlCommandService;
        _requestToHarService = requestToHarService;
    }

    /// <inheritdoc />
    public async Task<string> Handle(ExportRequestQuery request, CancellationToken cancellationToken)
    {
        var requestResult = await _stubContext.GetRequestResultAsync(request.CorrelationId, cancellationToken)
            .IfNull(() => throw new NotFoundException("request", request.CorrelationId));
        var response = await _stubContext.GetResponseAsync(request.CorrelationId, cancellationToken);
        switch (request.RequestExportType)
        {
            case RequestExportType.Curl:
                return _requestToCurlCommandService.Convert(requestResult);
            case RequestExportType.Har:
                if (response == null)
                {
                    throw new NotFoundException("response", request.CorrelationId);
                }

                return _requestToHarService.Convert(requestResult, response);
            default:
                throw new NotImplementedException(
                    $"Converting of request to {request.RequestExportType} not supported.");
        }
    }
}
