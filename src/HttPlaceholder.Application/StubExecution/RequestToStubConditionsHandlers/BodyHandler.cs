using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    internal class BodyHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
        {
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return Task.FromResult(false);
            }

            conditions.Body = new[] { request.Body };
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 1;
    }
}
