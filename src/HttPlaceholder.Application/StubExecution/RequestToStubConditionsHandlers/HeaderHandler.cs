using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    public class HeaderHandler : IRequestToStubConditionsHandler
    {
        private static readonly IEnumerable<string> _headersToStrip = new[] { "Postman-Token", "Host" };

        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
        {
            if (!request.Headers.Any())
            {
                return Task.FromResult(false);
            }

            // Do a Regex escape here, if we don do this it might give some strange results lateron
            // and filter some headers out.
            conditions.Headers = request.Headers
                .Where(h => !_headersToStrip.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(d => d.Key, d => Regex.Escape(d.Value));
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 1;
    }
}
