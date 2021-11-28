using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestStubGeneration
{
    /// <summary>
    /// Describes a class that is being used to set stub conditions based on a given HTTP request.
    /// </summary>
    public interface IRequestStubGenerationHandler
    {
        /// <summary>
        /// Handles the generation of a stub based on a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stub">The stub that is being generated.</param>
        /// <returns>True if the handler has been executed; false otherwise.</returns>
        Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub);

        /// <summary>
        /// A priority in which the handler should be executed. The higher the number, the earlier it is executed.
        /// </summary>
        int Priority { get; }
    }
}
