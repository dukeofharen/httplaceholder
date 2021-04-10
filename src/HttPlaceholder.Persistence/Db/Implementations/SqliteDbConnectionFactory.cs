using System.Data;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class SqliteDbConnectionFactory : IDbConnectionFactory
    {
        internal const string ConnectionStringKey = "Sqlite";
        private readonly IConfiguration _configuration;

        public SqliteDbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection() =>
            new SQLiteConnection(_configuration.GetConnectionString(ConnectionStringKey));
    }
}
