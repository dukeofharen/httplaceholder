using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// Describes a class that is being used to set the stub response based on a given HTTP response.
/// </summary>
public interface IResponseToStubResponseHandler
{
    /// <summary>
    /// Handles the generation of a stub based on a response
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="stubResponseModel">The response for the stub that is being generated.</param>
    /// <returns>True if the handler has been executed; false otherwise.</returns>
    Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel);

    /// <summary>
    /// A priority in which the handler should be executed. The higher the number, the earlier it is executed.
    /// </summary>
    int Priority { get; }
}
