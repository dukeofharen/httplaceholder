using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Signalling;

/// <summary>
///     Describes a class that is used to send messages to a stub hub.
/// </summary>
public interface IStubNotify
{
    /// <summary>
    ///     Handle the addition of a new stub.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StubAddedAsync(FullStubOverviewModel stub, CancellationToken cancellationToken);

    /// <summary>
    ///     Handle the deletion of a stub.
    /// </summary>
    /// <param name="stubId">The ID of the deleted stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StubDeletedAsync(string stubId, CancellationToken cancellationToken);
}
