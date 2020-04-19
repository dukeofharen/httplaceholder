using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class StatusCodeResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (response.StatusCode != 0)
            {
                return Task.FromResult(false);
            }

            response.StatusCode = stub.Response?.StatusCode ?? 200;
            return Task.FromResult(true);
        }
    }
}
