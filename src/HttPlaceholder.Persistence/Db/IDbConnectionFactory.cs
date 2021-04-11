using System.Data;

namespace HttPlaceholder.Persistence.Db
{
    /// <summary>
    /// Describes a class that creates a DB connection for a specific database engine.
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
