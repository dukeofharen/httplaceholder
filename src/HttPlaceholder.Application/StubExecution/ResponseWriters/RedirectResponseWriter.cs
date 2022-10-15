using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to setup a temporary or permanent redirect.
/// </summary>
internal class RedirectResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (stub.Response?.TemporaryRedirect != null)
        {
            var url = stub.Response.TemporaryRedirect;
            response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
            response.Headers.Add("Location", url);
            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }

        if (stub.Response?.PermanentRedirect != null)
        {
            var url = stub.Response.PermanentRedirect;
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.Headers.Add("Location", url);

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }

        return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
    }
}
