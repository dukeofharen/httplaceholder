using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters
{
    /// <summary>
    /// Describes a response writer that is used to populate the <see cref="ResponseModel"/> which is used for generating a response.
    /// </summary>
    public interface IResponseWriter
    {
        /// <summary>
        /// The implemented method writes data to the <see cref="response"/>.
        /// </summary>
        /// <param name="stub">The matched stub.</param>
        /// <param name="response">The response that will eventually be returned to the client.</param>
        /// <returns>A <see cref="StubResponseWriterResultModel"/> which contains data about the execution of this response writer.</returns>
        Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response);

        /// <summary>
        /// The priority of the given response writer. The higher the number, the earlier this response writer is executed.
        /// </summary>
        int Priority { get; }
    }
}
