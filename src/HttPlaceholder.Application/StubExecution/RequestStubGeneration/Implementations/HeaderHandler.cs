using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    public class HeaderHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            if (!request.RequestParameters.Headers.Any())
            {
                return Task.FromResult(false);
            }

            // Do a Regex escape here, if we don do this it might give some strange results lateron.
            stub.Conditions.Headers = request.RequestParameters.Headers
                .ToDictionary(d => d.Key, d => Regex.Escape(d.Value));
            return Task.FromResult(true);
        }

        public int Priority => 1;
    }
}
