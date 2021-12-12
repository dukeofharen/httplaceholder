using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <inheritdoc />
internal class MethodHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        if (string.IsNullOrWhiteSpace(request.Method))
        {
            throw new InvalidOperationException("No HTTP method set; this is unexpected.");
        }

        conditions.Method = request.Method;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}