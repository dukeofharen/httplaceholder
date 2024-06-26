using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create a query parameters condition.
/// </summary>
internal class QueryParamHandler : IRequestToStubConditionsHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var uri = new Uri(request.Url);
        var query = QueryHelpers.ParseQuery(uri.Query);
        if (query.Count == 0)
        {
            return false.AsTask();
        }

        conditions.Url.Query = query.ToDictionary(q => q.Key,
            q => new StubConditionStringCheckingModel { StringEquals = q.Value } as object);
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
