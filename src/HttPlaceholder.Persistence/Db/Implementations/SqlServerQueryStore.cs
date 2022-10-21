namespace HttPlaceholder.Persistence.Db.Implementations;

/// <summary>
///     A store that contains queries for working with MS SQL Server.
/// </summary>
internal class SqlServerQueryStore : IQueryStore
{
    /// <inheritdoc />
    public string GetRequestsQuery => @"SELECT
  id,
  correlation_id AS CorrelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  json,
  has_response AS HasResponse
FROM requests";

    /// <inheritdoc />
    public string GetRequestQuery => @"SELECT
  id,
  correlation_id AS CorrelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  json,
  has_response AS HasResponse
FROM requests
WHERE correlation_id = @CorrelationId";

    /// <inheritdoc />
    public string GetResponseQuery => @"
SELECT res.id             as Id,
       res.status_code    as StatusCode,
       res.headers        as Headers,
       res.body           as Body,
       res.body_is_binary as BodyIsBinary
FROM responses res
         LEFT JOIN requests req ON req.id = res.id
WHERE req.correlation_id = @CorrelationId;";

    /// <inheritdoc />
    public string DeleteAllRequestsQuery => @"DELETE FROM requests";

    /// <inheritdoc />
    public string DeleteRequestQuery => @"DELETE FROM requests WHERE correlation_id = @CorrelationId";

    /// <inheritdoc />
    public string AddRequestQuery => @"INSERT INTO requests
(correlation_id, executing_stub_id, request_begin_time, request_end_time, json, has_response)
VALUES (@CorrelationId, @ExecutingStubId, @RequestBeginTime, @RequestEndTime, @Json, @HasResponse)";

    /// <inheritdoc />
    public string AddResponseQuery => @"INSERT INTO responses (id, status_code, headers, body, body_is_binary)
VALUES ((SELECT id FROM requests WHERE correlation_id = @CorrelationId), @StatusCode, @Headers, @Body, @BodyIsBinary);";

    /// <inheritdoc />
    public string AddStubQuery => @"INSERT INTO stubs
(stub_id, stub, stub_type)
VALUES (@StubId, @Stub, @StubType)";

    /// <inheritdoc />
    public string DeleteStubQuery => @"DELETE FROM stubs WHERE stub_id = @StubId";

    /// <inheritdoc />
    public string GetStubsQuery => @"SELECT
stub_id AS StubId,
stub,
stub_type AS StubType
FROM stubs";

    /// <inheritdoc />
    public string GetStubQuery => @"SELECT
stub_id AS StubId,
stub,
stub_type AS StubType
FROM stubs
WHERE stub_id = @StubId";

    /// <inheritdoc />
    public string CleanOldRequestsQuery =>
        @"DELETE FROM requests WHERE ID NOT IN (SELECT TOP (@Limit) ID FROM requests ORDER BY ID DESC)";

    /// <inheritdoc />
    public string GetStubUpdateTrackingIdQuery => "SELECT stub_update_tracking_id FROM metadata";

    /// <inheritdoc />
    public string InsertStubUpdateTrackingIdQuery =>
        "INSERT INTO metadata (stub_update_tracking_id) VALUES (@StubUpdateTrackingId)";

    /// <inheritdoc />
    public string UpdateStubUpdateTrackingIdQuery =>
        "UPDATE metadata SET stub_update_tracking_id = @StubUpdateTrackingId";
}
