using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IRequestStubGenerator
    {
        Task<FullStubModel> GenerateStubBasedOnRequestAsync(string requestCorrelationId);
    }
}
