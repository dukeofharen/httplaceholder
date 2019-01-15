using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
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