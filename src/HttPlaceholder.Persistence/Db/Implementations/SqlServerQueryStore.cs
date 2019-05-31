using System.Data;
using System.Data.SqlClient;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.Persistence.Db.Resources;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class SqlServerQueryStore : IQueryStore
    {
        private readonly IConfigurationService _configurationService;

        public SqlServerQueryStore(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public string GetRequestsQuery => @"SELECT
  id,
  correlation_id AS CorelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  json
FROM requests";

        public string DeleteAllRequestsQuery => @"DELETE FROM requests";

        public string AddRequestQuery => @"INSERT INTO requests
(correlation_id, executing_stub_id, request_begin_time, request_end_time, json)
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

        public string CleanOldRequestsQuery => @"DELETE FROM requests WHERE ID NOT IN (SELECT TOP (@Limit) ID FROM requests ORDER BY ID DESC)";

        public string MigrationsQuery => SqlServerResources.MigrateScript;

        public IDbConnection GetConnection()
        {
            var config = _configurationService.GetConfiguration();
            string connectionString = config[Constants.ConfigKeys.SqlServerConnectionStringKey];
            return new SqlConnection(connectionString);
        }
    }
}
