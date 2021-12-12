using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

internal class RedirectResponseWriter : IResponseWriter
{
    public int Priority => 0;

    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
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