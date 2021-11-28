using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    public class ClientIpHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            stub.Conditions.ClientIp = request.RequestParameters.ClientIp;
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
