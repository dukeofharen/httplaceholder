﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// This handler is used for setting the stub response headers.
/// </summary>
internal class HeaderHandler : IResponseToStubResponseHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel, CancellationToken cancellationToken)
    {
        stubResponseModel.Headers = response.Headers;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
