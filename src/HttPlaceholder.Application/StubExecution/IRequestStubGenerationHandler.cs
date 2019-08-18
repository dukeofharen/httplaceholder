using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IRequestStubGenerationHandler
    {
        /// <summary>
        /// Handles the generation of a stub based on a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stub">The stub that is being generated.</param>
        /// <returns>True if the handler has been executed; false otherwise.</returns>
        Task<bool> HandleStubGenerationAsync(RequestResultModel request, StubModel stub);
    }
}
