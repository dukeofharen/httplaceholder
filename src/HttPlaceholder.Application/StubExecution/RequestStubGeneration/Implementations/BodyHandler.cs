using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class BodyHandler : IRequestStubGenerationHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            if (string.IsNullOrWhiteSpace(request.RequestParameters.Body))
            {
                return Task.FromResult(false);
            }

            stub.Conditions.Body = new[] {request.RequestParameters.Body};
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 1;
    }
}
