using System.Data;
using System.Data.SQLite;
using HttPlaceholder.Persistence.Db.Resources;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class SqliteQueryStore : IQueryStore
    {
        internal const string ConnectionStringKey = "Sqlite";
        private readonly IConfiguration _configuration;

        public SqliteQueryStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetRequestsQuery => @"SELECT
  id,
  correlation_id AS CorelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  `json`
FROM requests";

        public string GetRequestQuery => @"SELECT
  id,
  correlation_id AS CorelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  `json`
FROM requests
WHERE correlation_id = @CorrelationId";

        public string DeleteAllRequestsQuery => @"DELETE FROM requests";

        public string AddRequestQuery => @"INSERT INTO requests
(correlation_id, executing_stub_id, request_begin_time, request_end_time, `json`)
VALUES (@CorrelationId, @ExecutingStubid, @RequestBeginTime, @RequestEndTime, @Json)";

        public string AddStubQuery => @"INSERT INTO stubs
(stub_id, stub, stub_type)
VALUES (@StubId, @Stub, @StubType)";

        public string DeleteStubQuery => @"DELETE FROM stubs WHERE stub_id = @StubId";

        public string GetStubsQuery => @"SELECT
stub_id AS StubId,
stub,
stub_type AS StubType
FROM stubs";

        public string GetStubQuery => @"SELECT
stub_id AS StubId,
stub,
stub_type AS StubType
FROM stubs
WHERE stub_id = @StubId";

        public string CleanOldRequestsQuery => @"DELETE FROM requests WHERE ID NOT IN (SELECT * FROM (SELECT Id FROM requests ORDER BY Id DESC LIMIT 0,@Limit) AS t1)";

        public string MigrationsQuery => SqliteResources.MigrateScript;

        public IDbConnection GetConnection() =>
            new SQLiteConnection(_configuration.GetConnectionString(ConnectionStringKey));
    }
}
