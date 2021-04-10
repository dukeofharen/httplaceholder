
using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class MysqlDbConnectionFactory : IDbConnectionFactory
    {
        internal const string ConnectionStringKey = "MySql";
        private readonly IConfiguration _configuration;

        public MysqlDbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection() => new MySqlConnection(_configuration.GetConnectionString(ConnectionStringKey));
    }
}
