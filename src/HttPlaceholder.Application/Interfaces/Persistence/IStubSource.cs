using System.Collections.Generic;
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
    /// <returns>A list of <see cref="StubModel"/>.</returns>
    Task<IEnumerable<StubModel>> GetStubsAsync();

    /// <summary>
    /// Gets an overview list of <see cref="StubOverviewModel"/>.
    /// </summary>
    /// <returns>An overview list of <see cref="StubOverviewModel"/>.</returns>
    Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync();

    /// <summary>
    /// Gets a <see cref="StubModel"/> by ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>A <see cref="StubModel"/>.</returns>
    Task<StubModel> GetStubAsync(string stubId);

    /// <summary>
    /// Prepares a stub source (e.g. setup tables, create folders etc.).
    /// </summary>
    Task PrepareStubSourceAsync();
}
