using System.Data;
using System.Data.Common;

namespace HttPlaceholder.Persistence.Db;

/// <summary>
///     Describes a class that creates a DB connection for a specific database engine.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    ///     Creates an <see cref="IDbConnection" /> instance.
    /// </summary>
    /// <returns>An <see cref="IDbConnection" /> instance.</returns>
    IDbConnection GetConnection();


    /// <summary>
    ///     Creates a <see cref="DbDataSource"/> instance.
    /// </summary>
    /// <returns>A <see cref="DbDataSource"/> instance.</returns>
    DbDataSource GetDataSource();
}
