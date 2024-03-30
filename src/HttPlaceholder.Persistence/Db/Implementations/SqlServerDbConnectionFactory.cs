using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating MS SQL Server DB connections.
/// </summary>
internal class SqlServerDbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "SqlServer";

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new SqlConnection(configuration.GetConnectionString(ConnectionStringKey) ??
                          throw new InvalidOperationException("SQL Server connection string not found."));


    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
