using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// Describes a class that is being used to set stub conditions based on a given HTTP request.
/// </summary>
public interface IRequestToStubConditionsHandler
{
    /// <summary>
    /// Handles the generation of a stub based on a request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="conditions">The conditions for the stub that is being generated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the handler has been executed; false otherwise.</returns>
    Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions, CancellationToken cancellationToken);

    /// <summary>
    /// A priority in which the handler should be executed. The higher the number, the earlier it is executed.
    /// </summary>
    int Priority { get; }
}
