using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// A response writer that is used to abruptly abort a connection with the calling client.
/// </summary>
internal class AbortConnectionResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        var abortConnection = stub.Response?.AbortConnection == true;
        if (!abortConnection)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        response.AbortConnection = true;
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    /// <inheritdoc />
    public int Priority => 100;
}
