using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
/// A class for creating MS SQL Server DB connections.
/// </summary>
internal class SqlServerDbConnectionFactory : IDbConnectionFactory
{
    internal const string ConnectionStringKey = "SqlServer";
    private readonly IConfiguration _configuration;

    public SqlServerDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public IDbConnection GetConnection() =>
        new SqlConnection(_configuration.GetConnectionString(ConnectionStringKey));
}
