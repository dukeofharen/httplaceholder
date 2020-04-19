using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    internal class JsonResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var executed = false;
            if (stub.Response?.Json != null)
            {
                var jsonBody = stub.Response.Json;
                response.Body = Encoding.UTF8.GetBytes(jsonBody);
                if (!response.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    response.Headers.Add("Content-Type", "application/json");
                }

                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}
