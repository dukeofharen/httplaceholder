using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Persistence.FileStorage
{
    /// <summary>
    /// Describes a class which is used to keep track of the stub cache and invalidation.
    /// </summary>
    public interface IFileSystemStubCache
    {
        /// <summary>
        /// Loads the stub cache to memory if it has not been done yet, returns the current cache or updates the current cache if something has changed.
        /// </summary>
        /// <returns>A list of <see cref="StubModel"/>.</returns>
        IEnumerable<StubModel> GetOrUpdateStubCache();

        /// <summary>
        /// Clears the stub cache.
        /// </summary>
        void ClearStubCache();
    }
}
