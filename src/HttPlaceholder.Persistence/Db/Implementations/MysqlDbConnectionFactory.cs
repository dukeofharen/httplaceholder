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
        new MySqlConnection(configuration.GetConnectionString(ConnectionStringKey));

    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
