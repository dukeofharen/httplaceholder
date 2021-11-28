using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    internal class QueryParamHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub)
        {
            var uri = new Uri(request.RequestParameters.Url);
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (!query.Any())
            {
                return Task.FromResult(false);
            }

            stub.Conditions.Url.Query = query.ToDictionary(q => q.Key, q => q.Value.ToString());
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
