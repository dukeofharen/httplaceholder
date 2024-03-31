using System;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating MySQL DB connections.
/// </summary>
internal class MysqlDbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "MySql";

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new MySqlConnection(configuration.GetConnectionString(ConnectionStringKey) ??
                            throw new InvalidOperationException("MySQL connection string not found."));

    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
