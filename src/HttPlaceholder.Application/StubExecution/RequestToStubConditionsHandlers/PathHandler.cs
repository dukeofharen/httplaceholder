using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    internal class PathHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var uri = new Uri(request.RequestParameters.Url);
            stub.Conditions.Url.Path = uri.LocalPath;
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
