namespace HttPlaceholder.Persistence.Db;

/// <summary>
/// An interface that describes a class that returns queries for a specific database engine.
/// </summary>
public interface IQueryStore
{
    /// <summary>
    /// Returns the query for getting all requests.
    /// </summary>
    string GetRequestsQuery { get; }

    /// <summary>
    /// Returns the query for getting a specific request.
    /// </summary>
    string GetRequestQuery { get; }

    /// <summary>
    /// Returns the query for deleting all requests.
    /// </summary>
    string DeleteAllRequestsQuery { get; }

    /// <summary>
    /// Returns the query for deleting a specific request.
    /// </summary>
    string DeleteRequestQuery { get; }

    /// <summary>
    /// Returns the query for adding a request.
    /// </summary>
    string AddRequestQuery { get; }

    /// <summary>
    /// Returns the query for adding a stub.
    /// </summary>
    string AddStubQuery { get; }

    /// <summary>
    /// Returns the query for deleting a stub.
    /// </summary>
    string DeleteStubQuery { get; }

    /// <summary>
    /// Returns the query for getting all stubs.
    /// </summary>
    string GetStubsQuery { get; }

    /// <summary>
    /// Returns the query for getting a single stub.
    /// </summary>
    string GetStubQuery { get; }

    /// <summary>
    /// Returns the query for cleaning old requests.
    /// </summary>
    string CleanOldRequestsQuery { get; }

    /// <summary>
    /// Returns the query for getting the "stub update tracking ID".
    /// </summary>
    string GetStubUpdateTrackingIdQuery { get; }

    /// <summary>
    /// Returns the query for inserting the "stub update tracking ID".
    /// </summary>
    string InsertStubUpdateTrackingIdQuery { get; }

    /// <summary>
    /// Returns the query for updating the "stub update tracking ID".
    /// </summary>
    string UpdateStubUpdateTrackingIdQuery { get; }
}
