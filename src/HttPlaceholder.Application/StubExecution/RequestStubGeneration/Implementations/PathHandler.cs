using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    internal class PathHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var uri = new Uri(request.RequestParameters.Url);
            stub.Conditions.Url.Path = uri.LocalPath;
            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
