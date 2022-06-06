using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// "Request to stub conditions handler" that is used to create a query parameters condition.
/// </summary>
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

        conditions.Url.Query = query.ToDictionary(q => q.Key,
            q => new StubConditionStringCheckingModel {StringEquals = q.Value} as object);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
