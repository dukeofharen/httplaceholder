using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to return response headers.
/// </summary>
internal class HeadersResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => -11;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var stubResponseHeaders = stub.Response?.Headers;
        if (stubResponseHeaders == null || stubResponseHeaders.Any() != true)
        {
            return Task.FromResult(IsNotExecuted(GetType().Name));
        }

        foreach (var header in stubResponseHeaders)
        {
            response.Headers.AddOrReplaceCaseInsensitive(header.Key, header.Value);
        }

        return Task.FromResult(IsExecuted(GetType().Name));
    }
}
