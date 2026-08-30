using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create a request body condition.
/// </summary>
internal class BodyHandler : IRequestToStubConditionsHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        if (conditions.Json != null || string.IsNullOrWhiteSpace(request.Body))
        {
            return false.AsTask();
        }

        conditions.Body = new[] { new StubConditionStringCheckingModel { StringEquals = request.Body } };
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 1;
}
