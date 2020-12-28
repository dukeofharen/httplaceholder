using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ContentTypeResponseWriter : IResponseWriter
    {
        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (string.IsNullOrWhiteSpace(stub.Response?.ContentType))
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            var pair = response.Headers.FirstOrDefault(h => string.Equals("content-type", h.Key));
            if (!string.IsNullOrWhiteSpace(pair.Key))
            {
                response.Headers.Remove(pair.Key);
            }

            response.Headers.Add("Content-Type", stub.Response.ContentType);
            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }

        public int Priority { get; } = -11;
    }
}
