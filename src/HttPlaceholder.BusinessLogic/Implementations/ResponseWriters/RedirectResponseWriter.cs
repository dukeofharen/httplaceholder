using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    internal class RedirectResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (stub.Response?.TemporaryRedirect != null)
            {
                string url = stub.Response.TemporaryRedirect;
                response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
                response.Headers.Add("Location", url);
                executed = true;
            }

            if (stub.Response?.PermanentRedirect != null)
            {
                string url = stub.Response.PermanentRedirect;
                response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                response.Headers.Add("Location", url);
                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}