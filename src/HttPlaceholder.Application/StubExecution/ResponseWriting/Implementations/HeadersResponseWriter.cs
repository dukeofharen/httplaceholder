using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class HeadersResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var stubResponseHeaders = stub.Response?.Headers;
            if (stubResponseHeaders == null || stubResponseHeaders?.Any() != true)
            {
                return Task.FromResult(false);
            }

            foreach (var header in stubResponseHeaders)
            {
                response.Headers.Add(header.Key, header.Value);
            }

            return Task.FromResult(true);
        }
    }
}
