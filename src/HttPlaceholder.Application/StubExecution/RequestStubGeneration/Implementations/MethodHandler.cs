using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class MethodHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            if (string.IsNullOrWhiteSpace(request.RequestParameters.Method))
            {
                throw new InvalidOperationException($"No HTTP method set; this is unexpected.");
            }

            stub.Conditions.Method = request.RequestParameters.Method;
            return Task.FromResult(true);
        }
    }
}
