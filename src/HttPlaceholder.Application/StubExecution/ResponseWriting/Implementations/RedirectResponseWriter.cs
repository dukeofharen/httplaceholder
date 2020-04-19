using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    internal class RedirectResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var executed = false;
            if (stub.Response?.TemporaryRedirect != null)
            {
                var url = stub.Response.TemporaryRedirect;
                response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
                response.Headers.Add("Location", url);
                executed = true;
            }

            if (stub.Response?.PermanentRedirect != null)
            {
                var url = stub.Response.PermanentRedirect;
                response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                response.Headers.Add("Location", url);
                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}