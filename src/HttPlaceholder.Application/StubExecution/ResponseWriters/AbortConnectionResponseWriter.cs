using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     A response writer that is used to abruptly abort a connection with the calling client.
/// </summary>
internal class AbortConnectionResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var abortConnection = stub.Response?.AbortConnection == true;
        if (!abortConnection)
        {
            return IsNotExecuted(GetType().Name).AsTask();
        }

        response.AbortConnection = true;
        return IsExecuted(GetType().Name).AsTask();
    }

    /// <inheritdoc />
    public int Priority => 100;
}
