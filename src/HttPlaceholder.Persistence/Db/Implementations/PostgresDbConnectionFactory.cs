using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A class for creating Postgres DB connections.
/// </summary>
internal class PostgresDbConnectionFactory : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "Postgres";
    private readonly IConfiguration _configuration;

    public PostgresDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public IDbConnection GetConnection() => null;

    public DbDataSource GetDataSource() =>
        NpgsqlDataSource.Create(_configuration.GetConnectionString(ConnectionStringKey));
}
