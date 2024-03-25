using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

/// <summary>
///     Describes a class that is used to implement a stub source type.
/// </summary>
public interface IStubSource
{
    /// <summary>
    ///     Gets a list of <see cref="StubModel" /> with a dictionary of possible metadata.
    /// </summary>
    /// <param name="distributionKey">
    ///     The distribution key the stubs should be retrieved for. Leave it null if there is no
    ///     user.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="StubModel" /> with the corresponding metadata.</returns>
    Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an overview list of <see cref="StubOverviewModel" /> with a dictionary of possible metadata.
    /// </summary>
    /// <param name="distributionKey">
    ///     The distribution key the stubs should be retrieved for. Leave it null if there is no
    ///     user.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An overview list of <see cref="StubOverviewModel" /> with the corresponding metadata.</returns>
    Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>> GetStubsOverviewAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a <see cref="StubModel" /> by ID with a dictionary of possible metadata.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="distributionKey">The distribution key the stub should be retrieved for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="StubModel" /> with the corresponding metadata.</returns>
    Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Prepares a stub source (e.g. setup tables, create folders etc.).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PrepareStubSourceAsync(CancellationToken cancellationToken);
}
