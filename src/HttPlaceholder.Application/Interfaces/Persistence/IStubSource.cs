using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

/// <summary>
/// Describes a class that is used to implement a stub source type.
/// </summary>
public interface IStubSource
{
    /// <summary>
    /// Gets a list of <see cref="StubModel"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="StubModel"/>.</returns>
    Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets an overview list of <see cref="StubOverviewModel"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An overview list of <see cref="StubOverviewModel"/>.</returns>
    Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a <see cref="StubModel"/> by ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="StubModel"/>.</returns>
    Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken);

    /// <summary>
    /// Prepares a stub source (e.g. setup tables, create folders etc.).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PrepareStubSourceAsync(CancellationToken cancellationToken);
}
