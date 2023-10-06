using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating MySQL DB connections.
/// </summary>
internal class MysqlDbConnectionFactory : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "MySql";
    private readonly IConfiguration _configuration;

    public MysqlDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new MySqlConnection(_configuration.GetConnectionString(ConnectionStringKey));

    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
