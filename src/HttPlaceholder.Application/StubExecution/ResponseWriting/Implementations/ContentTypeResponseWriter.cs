using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ContentTypeResponseWriter : IResponseWriter
    {
        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response) => throw new System.NotImplementedException();

        public int Priority { get; } = -11;
    }
}
