using System.Data;

namespace HttPlaceholder.DataLogic.Db
{
    /// <summary>
    /// An interface that describes a class that returns queries for a specific database engine.
    /// </summary>
    public interface IQueryStore
    {
        IDbConnection GetConnection();

        string GetRequestsQuery { get; }
    }
}
