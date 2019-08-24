using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    public class ClientIpHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            stub.Conditions.ClientIp = request.RequestParameters.ClientIp;
            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
