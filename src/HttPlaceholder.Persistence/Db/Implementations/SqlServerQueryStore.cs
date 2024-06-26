﻿namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A store that contains queries for working with MS SQL Server.
/// </summary>
internal class SqlServerQueryStore : IQueryStore
{
    /// <inheritdoc />
    public string GetRequestsQuery => """
                                      SELECT
                                        id,
                                        correlation_id AS CorrelationId,
                                        executing_stub_id AS ExecutingStubId,
                                        request_begin_time AS RequestBeginTime,
                                        request_end_time AS RequestEndTime,
                                        json,
                                        has_response AS HasResponse
                                      FROM requests
                                      WHERE distribution_key = @DistributionKey
                                      ORDER BY request_begin_time DESC
                                      """;

    /// <inheritdoc />
    public string GetRequestsByCorrelationIdsQuery => """
                                                      SELECT
                                                        id,
                                                        correlation_id AS CorrelationId,
                                                        executing_stub_id AS ExecutingStubId,
                                                        request_begin_time AS RequestBeginTime,
                                                        request_end_time AS RequestEndTime,
                                                        json,
                                                        has_response AS HasResponse
                                                      FROM requests
                                                      WHERE correlation_id IN @CorrelationIds
                                                      AND distribution_key = @DistributionKey
                                                      ORDER BY request_begin_time DESC
                                                      """;

    /// <inheritdoc />
    public string GetPagedRequestCorrelationIdsQuery =>
        "SELECT correlation_id FROM requests WHERE distribution_key = @DistributionKey ORDER BY request_begin_time DESC";

    /// <inheritdoc />
    public string GetRequestQuery => """
                                     SELECT
                                       id,
                                       correlation_id AS CorrelationId,
                                       executing_stub_id AS ExecutingStubId,
                                       request_begin_time AS RequestBeginTime,
                                       request_end_time AS RequestEndTime,
                                       json,
                                       has_response AS HasResponse
                                     FROM requests
                                     WHERE correlation_id = @CorrelationId
                                     AND distribution_key = @DistributionKey
                                     """;

    /// <inheritdoc />
    public string GetResponseQuery => """

                                      SELECT res.id             as Id,
                                             res.status_code    as StatusCode,
                                             res.headers        as Headers,
                                             res.body           as Body,
                                             res.body_is_binary as BodyIsBinary
                                      FROM responses res
                                               LEFT JOIN requests req ON req.id = res.id
                                      WHERE req.correlation_id = @CorrelationId
                                      AND req.distribution_key = @DistributionKey;
                                      """;

    /// <inheritdoc />
    public string DeleteAllRequestsQuery => "DELETE FROM requests WHERE distribution_key = @DistributionKey";

    /// <inheritdoc />
    public string DeleteRequestQuery =>
        "DELETE FROM requests WHERE correlation_id = @CorrelationId AND distribution_key = @DistributionKey";

    /// <inheritdoc />
    public string AddRequestQuery => """
                                     INSERT INTO requests
                                     (correlation_id, executing_stub_id, request_begin_time, request_end_time, json, has_response, distribution_key)
                                     VALUES (@CorrelationId, @ExecutingStubId, @RequestBeginTime, @RequestEndTime, @Json, @HasResponse, @DistributionKey)
                                     """;

    /// <inheritdoc />
    public string AddResponseQuery =>
        """
        INSERT INTO responses (id, status_code, headers, body, body_is_binary, distribution_key)
        VALUES ((SELECT id FROM requests WHERE correlation_id = @CorrelationId), @StatusCode, @Headers, @Body, @BodyIsBinary, @DistributionKey);
        """;

    /// <inheritdoc />
    public string AddStubQuery => """
                                  INSERT INTO stubs
                                  (stub_id, stub, stub_type, distribution_key)
                                  VALUES (@StubId, @Stub, @StubType, @DistributionKey)
                                  """;

    /// <inheritdoc />
    public string DeleteStubQuery =>
        "DELETE FROM stubs WHERE stub_id = @StubId AND distribution_key = @DistributionKey";

    /// <inheritdoc />
    public string GetStubsQuery => """
                                   SELECT
                                   stub_id AS StubId,
                                   stub,
                                   stub_type AS StubType
                                   FROM stubs
                                   WHERE distribution_key = @DistributionKey
                                   """;

    /// <inheritdoc />
    public string GetStubQuery => """
                                  SELECT
                                  stub_id AS StubId,
                                  stub,
                                  stub_type AS StubType
                                  FROM stubs
                                  WHERE stub_id = @StubId
                                  AND distribution_key = @DistributionKey
                                  """;

    /// <inheritdoc />
    public string CleanOldRequestsQuery =>
        "DELETE FROM requests WHERE ID NOT IN (SELECT TOP (@Limit) ID FROM requests WHERE distribution_key = @DistributionKey ORDER BY ID DESC)";

    /// <inheritdoc />
    public string GetStubUpdateTrackingIdQuery => "SELECT stub_update_tracking_id FROM metadata";

    /// <inheritdoc />
    public string InsertStubUpdateTrackingIdQuery =>
        "INSERT INTO metadata (stub_update_tracking_id) VALUES (@StubUpdateTrackingId)";

    /// <inheritdoc />
    public string UpdateStubUpdateTrackingIdQuery =>
        "UPDATE metadata SET stub_update_tracking_id = @StubUpdateTrackingId";

    /// <inheritdoc />
    public string GetDistinctRequestDistributionKeysQuery => "SELECT DISTINCT(distribution_key) FROM requests;";

    /// <inheritdoc />
    public string GetScenarioQuery => """
                                      SELECT scenario AS Scenario, state AS State, hit_count AS HitCount
                                      FROM scenarios
                                      WHERE scenario = @Scenario
                                      AND distribution_key = @DistributionKey
                                      """;

    /// <inheritdoc />
    public string AddScenarioQuery => """
                                      INSERT INTO scenarios
                                      (distribution_key, scenario, state, hit_count)
                                      VALUES (@DistributionKey, @Scenario, @State, @HitCount)
                                      """;

    /// <inheritdoc />
    public string UpdateScenarioQuery => """
                                         UPDATE scenarios SET state = @State, hit_count = @HitCount
                                         WHERE scenario = @Scenario
                                         AND distribution_key = @DistributionKey
                                         """;

    /// <inheritdoc />
    public string GetAllScenariosQuery => """
                                          SELECT scenario AS Scenario, state AS State, hit_count AS HitCount
                                          FROM scenarios
                                          WHERE distribution_key = @DistributionKey
                                          """;

    /// <inheritdoc />
    public string DeleteScenarioQuery => """
                                         DELETE FROM scenarios
                                         WHERE scenario = @Scenario
                                         AND distribution_key = @DistributionKey
                                         """;

    public string DeleteAllScenariosQuery => """
                                             DELETE FROM scenarios
                                             WHERE distribution_key = @DistributionKey
                                             """;
}
