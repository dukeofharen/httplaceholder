using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting
{
    public interface IResponseWriter
    {
        Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response);

        int Priority { get; }
    }
}
