using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers
{
    /// <inheritdoc />
    internal class QueryParamHandler : IRequestToStubConditionsHandler
    {
        /// <inheritdoc />
        public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
        {
            var uri = new Uri(request.Url);
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (!query.Any())
            {
                return Task.FromResult(false);
            }

            conditions.Url.Query = query.ToDictionary(q => q.Key, q => q.Value.ToString());
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public int Priority => 0;
    }
}
