namespace HttPlaceholder.Persistence.Db;

/// <summary>
/// An interface that describes a class that returns queries for a specific database engine.
/// </summary>
public interface IQueryStore
{
    string GetRequestsQuery { get; }

    string GetRequestQuery { get; }

    string DeleteAllRequestsQuery { get; }

    string DeleteRequestQuery { get; }

    string AddRequestQuery { get; }

    string AddStubQuery { get; }

    string DeleteStubQuery { get; }

    string GetStubsQuery { get; }

    string GetStubQuery { get; }

    string CleanOldRequestsQuery { get; }

    string GetStubUpdateTrackingIdQuery { get; }

    string InsertStubUpdateTrackingIdQuery { get; }

    string UpdateStubUpdateTrackingIdQuery { get; }

    string MigrationsQuery { get; }
}