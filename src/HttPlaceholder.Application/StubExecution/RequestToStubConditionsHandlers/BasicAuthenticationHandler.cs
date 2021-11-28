using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    public class BasicAuthenticationHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
        {
            var pair = request.Headers.FirstOrDefault(p =>
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

            conditions.BasicAuthentication = new StubBasicAuthenticationModel
            {
                Username = parts[0],
                Password = parts[1]
            };

            // Make sure the original Authorization header is removed here.
            conditions.Headers = conditions.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(d => d.Key, d => d.Value);

            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}