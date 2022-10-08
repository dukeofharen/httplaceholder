using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used for storing the request body as base64. When the response writer is used, the response is base64 decoded and sent to the client.
/// </summary>
internal class Base64ResponseWriter : IResponseWriter
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (stub.Response?.Base64 == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var base64Body = stub.Response.Base64;
        response.Body = Convert.FromBase64String(base64Body);
        response.BodyIsBinary = true;
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
