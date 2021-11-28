using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    internal class BodyHandler : IRequestToStubConditionsHandler
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
