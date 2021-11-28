using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    /// <inheritdoc />
    internal class MethodHandler : IRequestStubGenerationHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            if (string.IsNullOrWhiteSpace(request.RequestParameters.Method))
            {
                throw new InvalidOperationException("No HTTP method set; this is unexpected.");
            }

            stub.Conditions.Method = request.RequestParameters.Method;
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
