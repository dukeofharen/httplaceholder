using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating SQLite DB connections.
/// </summary>
internal class SqliteDbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "Sqlite";

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new SQLiteConnection(configuration.GetConnectionString(ConnectionStringKey) ??
                             throw new InvalidOperationException(PersistenceResources.SqliteStringNotFound));

    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
