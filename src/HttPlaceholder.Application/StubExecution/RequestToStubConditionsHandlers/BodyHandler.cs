using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// "Request to stub conditions handler" that is used to create a request body condition.
/// </summary>
internal class BodyHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        if (conditions.Json != null)
        {
            return Task.FromResult(false);
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return Task.FromResult(false);
        }

        conditions.Body = new[] {new StubConditionStringCheckingModel {StringEquals = request.Body}};
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 1;
}
