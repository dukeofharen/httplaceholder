using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    public class HostHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var uri = new Uri(request.RequestParameters.Url);
            string host = uri.Host;
            if (uri.Port != 80 && uri.Port != 443)
            {
                host += $":{uri.Port}";
            }
            
            stub.Conditions.Host = host;
            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
