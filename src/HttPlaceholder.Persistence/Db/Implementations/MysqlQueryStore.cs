using System.Data;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.Persistence.Db.Resources;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class MysqlQueryStore : IQueryStore
    {
        internal const string ConnectionStringKey = "MySql";
        private readonly IConfiguration _configuration;

        public MysqlQueryStore(IConfiguration configuration)
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

        public string DeleteAllRequestsQuery => @"DELETE FROM requests";

        public string AddRequestQuery => @"INSERT INTO requests
(correlation_id, executing_stub_id, request_begin_time, request_end_time, `json`)
VALUES (@CorrelationId, @ExecutingStubid, @RequestBeginTime, @RequestEndTime, @Json)";

        public string AddStubQuery => @"INSERT INTO stubs
(stub_id, stub, stub_type)
VALUES (@StubId, @Stub, @StubType)";

        public string DeletStubQuery => @"DELETE FROM stubs WHERE stub_id = @StubId";

        public string GetStubsQuery => @"SELECT
stub_id AS StubId,
stub,
stub_type AS StubType
FROM stubs";

        public string CleanOldRequestsQuery => @"DELETE FROM requests WHERE ID NOT IN (SELECT * FROM (SELECT Id FROM requests ORDER BY Id DESC LIMIT 0,@Limit) AS t1)";

        public string MigrationsQuery => MysqlResources.MigrateScript;

        public IDbConnection GetConnection() =>
            new MySqlConnection(_configuration.GetConnectionString(ConnectionStringKey));
    }
}
