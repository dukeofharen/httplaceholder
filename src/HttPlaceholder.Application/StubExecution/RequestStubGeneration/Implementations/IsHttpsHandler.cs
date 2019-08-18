using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class IsHttpsHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            stub.Conditions.Url.IsHttps =
                request.RequestParameters.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
