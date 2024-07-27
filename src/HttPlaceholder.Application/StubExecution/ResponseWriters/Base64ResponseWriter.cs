using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used for storing the request body as base64. When the response writer is used, the response
///     is base64 decoded and sent to the client.
/// </summary>
internal class Base64ResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (stub.Response?.Base64 == null)
        {
            return IsNotExecuted(GetType().Name).AsTask();
        }

        response.Body = Convert.FromBase64String(stub.Response.Base64);
        var contentType = stub.GetContentType();
        response.BodyIsBinary = !response.Body.IsValidText(contentType);
        return IsExecuted(GetType().Name).AsTask();
    }
}
