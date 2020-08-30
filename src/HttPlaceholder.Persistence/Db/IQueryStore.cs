using System.Data;

namespace HttPlaceholder.Persistence.Db
{
    /// <summary>
    /// An interface that describes a class that returns queries and other related objects for a specific database engine.
    /// </summary>
    public interface IQueryStore
    {
        IDbConnection GetConnection();

        string GetRequestsQuery { get; }

        string GetRequestQuery { get; }

        string DeleteAllRequestsQuery { get; }

        string AddRequestQuery { get; }

        string AddStubQuery { get; }

        string DeleteStubQuery { get; }

        string GetStubsQuery { get; }

        string CleanOldRequestsQuery { get; }

        string MigrationsQuery { get; }
    }
}
