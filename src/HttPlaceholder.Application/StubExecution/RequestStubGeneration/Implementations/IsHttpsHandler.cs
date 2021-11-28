using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    /// <inheritdoc />
    internal class IsHttpsHandler : IRequestStubGenerationHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            stub.Conditions.Url.IsHttps =
                request.RequestParameters.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
