using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to set the HTTP status code.
/// </summary>
internal class StatusCodeResponseWriter : IResponseWriter
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (response.StatusCode != 0)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        response.StatusCode = stub.Response?.StatusCode ?? 200;
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
