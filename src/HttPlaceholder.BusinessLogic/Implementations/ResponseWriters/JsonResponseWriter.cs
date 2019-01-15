using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    internal class JsonResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (stub.Response?.Json != null)
            {
                string jsonBody = stub.Response.Json;
                response.Body = Encoding.UTF8.GetBytes(jsonBody);
                string bodyForLogging = jsonBody.Length > 10 ? jsonBody.Substring(0, 10) : jsonBody;
                if (!response.Headers.TryGetValue("Content-Type", out string contentType))
                {
                    response.Headers.Add("Content-Type", "application/json");
                }

                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}