using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    public class HostHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var uri = new Uri(request.RequestParameters.Url);
            var host = uri.Host;
            if (uri.Port != 80 && uri.Port != 443)
            {
                host += $":{uri.Port}";
            }

            stub.Conditions.Host = host;
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
