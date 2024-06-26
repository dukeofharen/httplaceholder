﻿using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to setup a temporary or permanent redirect.
/// </summary>
internal class RedirectResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken) =>
        (Redirect(stub.Response?.TemporaryRedirect, HttpStatusCode.TemporaryRedirect, response) ??
         Redirect(stub.Response?.PermanentRedirect, HttpStatusCode.PermanentRedirect, response) ??
         Redirect(stub.Response?.MovedPermanently, HttpStatusCode.MovedPermanently, response) ??
         IsNotExecuted(GetType().Name)).AsTask();

    private StubResponseWriterResultModel Redirect(string url, HttpStatusCode httpStatusCode, ResponseModel response)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        response.StatusCode = (int)httpStatusCode;
        response.Headers.Add(HeaderKeys.Location, url);
        return IsExecuted(GetType().Name);
    }
}
