using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration.Implementations
{
    public class BasicAuthenticationHandler : IRequestStubGenerationHandler
    {
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var pair = request.RequestParameters.Headers.FirstOrDefault(p =>
                p.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(pair.Value) || !pair.Value.Trim().ToLower().StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            var value = pair.Value;
            value = value.Replace("Basic ", string.Empty);
            var basicAuth = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            var parts = basicAuth.Split(':');
            if (parts.Length != 2)
            {
                return Task.FromResult(false);
            }

            stub.Conditions.BasicAuthentication = new StubBasicAuthenticationModel
            {
                Username = parts[0],
                Password = parts[1]
            };

            // Make sure the original Authorization header is removed here.
            stub.Conditions.Headers = stub.Conditions.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(d => d.Key, d => d.Value);

            return Task.FromResult(true);
        }

        public int Priority => 0;
    }
}
