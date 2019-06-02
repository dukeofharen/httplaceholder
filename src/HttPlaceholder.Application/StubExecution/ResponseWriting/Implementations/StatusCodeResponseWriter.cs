using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class StatusCodeResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (response.StatusCode == 0)
            {
                response.StatusCode = stub.Response?.StatusCode ?? 200;
                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}