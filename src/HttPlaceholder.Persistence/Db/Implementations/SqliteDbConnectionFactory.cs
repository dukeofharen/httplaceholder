using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating SQLite DB connections.
/// </summary>
internal class SqliteDbConnectionFactory : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "Sqlite";
    private readonly IConfiguration _configuration;

    public SqliteDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new SQLiteConnection(_configuration.GetConnectionString(ConnectionStringKey));

    /// <inheritdoc />
    public DbDataSource GetDataSource() => null;
}
