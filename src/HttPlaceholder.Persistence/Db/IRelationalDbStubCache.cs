﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Persistence.Db;

/// <summary>
/// A class which is used to keep track of the stub cache and invalidation for relational databases.
/// </summary>
public interface IRelationalDbStubCache
{
    /// <summary>
    /// Loads the stub cache to memory if it has not been done yet, returns the current cache or updates the current cache if something has changed.
    /// </summary>
    /// <param name="ctx">The database context.</param>
    /// <returns>A list of <see cref="StubModel"/>.</returns>
    Task<IEnumerable<StubModel>> GetOrUpdateStubCache(IDatabaseContext ctx);

    /// <summary>
    /// Clears the stub cache.
    /// </summary>
    /// <param name="ctx">The database context.</param>
    void ClearStubCache(IDatabaseContext ctx);
}
