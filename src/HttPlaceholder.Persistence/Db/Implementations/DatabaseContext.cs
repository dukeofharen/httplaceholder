using System.Data;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class DatabaseContext : IDatabaseContext
    {
        private readonly IDbConnection _dbConnection;

        public DatabaseContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Dispose() => _dbConnection?.Dispose();
    }
}
