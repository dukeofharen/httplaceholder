using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        internal const string ConnectionStringKey = "SqlServer";
        private readonly IConfiguration _configuration;

        public SqlServerDbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection() =>
            new SqlConnection(_configuration.GetConnectionString(ConnectionStringKey));
    }
}
