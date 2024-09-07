using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating Postgres DB connections.
/// </summary>
internal class PostgresDbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "Postgres";

    /// <inheritdoc />
    public IDbConnection GetConnection() => null;

    public DbDataSource GetDataSource() =>
        NpgsqlDataSource.Create(configuration.GetConnectionString(ConnectionStringKey) ??
                                throw new InvalidOperationException(PersistenceResources.PostgresStringNotFound));
}
