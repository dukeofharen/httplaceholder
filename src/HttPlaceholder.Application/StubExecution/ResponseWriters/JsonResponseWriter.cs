using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to return a given response as JSON.
/// </summary>
internal class JsonResponseWriter : IResponseWriter
{
    public int Priority => 0;

    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response?.Json == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var jsonBody = stub.Response.Json;
        response.Body = Encoding.UTF8.GetBytes(jsonBody);
        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", "application/json", false);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
