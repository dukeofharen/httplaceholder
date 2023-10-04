using System.Data;
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
    public IDbConnection GetConnection()
    {
        var dataSource = NpgsqlDataSource.Create(_configuration.GetConnectionString(ConnectionStringKey));
        return dataSource.CreateConnection();
    }
}
