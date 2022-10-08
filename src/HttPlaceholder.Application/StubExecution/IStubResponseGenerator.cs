using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to generate a <see cref="ResponseModel"/> based on a <see cref="StubModel"/>.
/// </summary>
public interface IStubResponseGenerator
{
    /// <summary>
    /// Generate a <see cref="ResponseModel"/> based on a <see cref="StubModel"/>.
    /// </summary>
    /// <param name="stub">The <see cref="StubModel"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="ResponseModel"/>.</returns>
    Task<ResponseModel> GenerateResponseAsync(StubModel stub, CancellationToken cancellationToken);
}
