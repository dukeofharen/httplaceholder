using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Persistence.FileSystem;

/// <summary>
///     Describes a class which is used to keep track of the stub cache and invalidation for file system stub sources.
/// </summary>
public interface IFileSystemStubCache
{
    /// <summary>
    ///     Loads the stub cache to memory if it has not been done yet, returns the current cache or updates the current cache
    ///     if something has changed.
    /// </summary>
    /// <param name="distributionKey">The distribution key the stubs should be added for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of <see cref="StubModel" />.</returns>
    Task<IEnumerable<StubModel>> GetOrUpdateStubCacheAsync(string distributionKey, CancellationToken cancellationToken);

    /// <summary>
    ///     Adds or updates a stub in the cache.
    /// </summary>
    /// <param name="stubModel">The stub to be added or updated.</param>
    void AddOrReplaceStub(StubModel stubModel);

    /// <summary>
    ///     Deletes a stub from the cache.
    /// </summary>
    /// <param name="stubId">The ID of the stub that should be deleted.</param>
    void DeleteStub(string stubId);
}
