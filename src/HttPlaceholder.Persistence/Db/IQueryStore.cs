﻿namespace HttPlaceholder.Persistence.Db;

/// <summary>
///     An interface that describes a class that returns queries for a specific database engine.
/// </summary>
public interface IQueryStore
{
    /// <summary>
    ///     Returns the query for getting all requests.
    /// </summary>
    string GetRequestsQuery { get; }

    /// <summary>
    ///     Returns the query for getting all requests for a list of correlation IDs.
    /// </summary>
    string GetRequestsByCorrelationIdsQuery { get; }

    /// <summary>
    ///     Returns the query that is needed to return the correlation IDs that are used for making a paged list of requests.
    /// </summary>
    string GetPagedRequestCorrelationIdsQuery { get; }

    /// <summary>
    ///     Returns the query for getting a specific request.
    /// </summary>
    string GetRequestQuery { get; }

    /// <summary>
    ///     Returns the query for getting a specific response.
    /// </summary>
    string GetResponseQuery { get; }

    /// <summary>
    ///     Returns the query for deleting all requests.
    /// </summary>
    string DeleteAllRequestsQuery { get; }

    /// <summary>
    ///     Returns the query for deleting a specific request.
    /// </summary>
    string DeleteRequestQuery { get; }

    /// <summary>
    ///     Returns the query for adding a request.
    /// </summary>
    string AddRequestQuery { get; }

    /// <summary>
    ///     Returns the query for adding a response.
    /// </summary>
    string AddResponseQuery { get; }

    /// <summary>
    ///     Returns the query for adding a stub.
    /// </summary>
    string AddStubQuery { get; }

    /// <summary>
    ///     Returns the query for deleting a stub.
    /// </summary>
    string DeleteStubQuery { get; }

    /// <summary>
    ///     Returns the query for getting all stubs.
    /// </summary>
    string GetStubsQuery { get; }

    /// <summary>
    ///     Returns the query for getting a single stub.
    /// </summary>
    string GetStubQuery { get; }

    /// <summary>
    ///     Returns the query for cleaning old requests.
    /// </summary>
    string CleanOldRequestsQuery { get; }

    /// <summary>
    ///     Returns the query for getting the "stub update tracking ID".
    /// </summary>
    string GetStubUpdateTrackingIdQuery { get; }

    /// <summary>
    ///     Returns the query for inserting the "stub update tracking ID".
    /// </summary>
    string InsertStubUpdateTrackingIdQuery { get; }

    /// <summary>
    ///     Returns the query for updating the "stub update tracking ID".
    /// </summary>
    string UpdateStubUpdateTrackingIdQuery { get; }

    /// <summary>
    ///     Returns the query for retrieving a list of unique distribution keys in the requests table.
    /// </summary>
    string GetDistinctRequestDistributionKeysQuery { get; }

    /// <summary>
    ///     Returns the query for retrieving a scenario.
    /// </summary>
    string GetScenarioQuery { get; }

    /// <summary>
    ///     Returns the query for adding a scenario.
    /// </summary>
    string AddScenarioQuery { get; }

    /// <summary>
    ///     Returns the query for updating a scenario.
    /// </summary>
    string UpdateScenarioQuery { get; }

    /// <summary>
    ///     Returns the query for retrieving all scenarios.
    /// </summary>
    string GetAllScenariosQuery { get; }

    /// <summary>
    ///     Returns the query for deleting a scenario.
    /// </summary>
    string DeleteScenarioQuery { get; }

    /// <summary>
    ///     Returns the query for deleting all scenarios.
    /// </summary>
    string DeleteAllScenariosQuery { get; }
}
