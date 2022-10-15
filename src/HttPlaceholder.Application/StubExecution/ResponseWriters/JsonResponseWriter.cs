using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to return a given response as JSON.
/// </summary>
internal class JsonResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (stub.Response?.Json == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var jsonBody = stub.Response.Json;
        response.Body = Encoding.UTF8.GetBytes(jsonBody);
        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", Constants.JsonMime, false);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
