using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
    public interface IResponseWriter
    {
        Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response);

        int Priority { get; }
    }
}