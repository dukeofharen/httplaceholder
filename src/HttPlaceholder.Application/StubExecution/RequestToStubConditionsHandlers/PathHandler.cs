using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <inheritdoc />
internal class PathHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        var uri = new Uri(request.Url);
        conditions.Url.Path = uri.LocalPath;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}