using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting
{
    public interface IResponseWriter
    {
        Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response);

        int Priority { get; }
    }
}
