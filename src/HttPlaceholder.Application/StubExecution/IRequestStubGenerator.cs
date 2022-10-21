using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is being used to generate a stub based on a previously executed request.
/// </summary>
public interface IRequestStubGenerator
{
    /// <summary>
    ///     Generate a stub based on a previously made request.
    /// </summary>
    /// <param name="requestCorrelationId">The correlation ID of the HTTP request.</param>
    /// <param name="doNotCreateStub">
    ///     If set to true, the stub will not directly be added to the underlying storage but only returned.
    ///     If set to false, will also add it to the underlying storage.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added <see cref="FullStubModel" />.</returns>
    Task<FullStubModel> GenerateStubBasedOnRequestAsync(string requestCorrelationId, bool doNotCreateStub,
        CancellationToken cancellationToken);
}
