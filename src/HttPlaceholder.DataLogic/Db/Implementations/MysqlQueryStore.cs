using System.Data;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using MySql.Data.MySqlClient;

namespace HttPlaceholder.DataLogic.Db.Implementations
{
    internal class MysqlQueryStore : IQueryStore
    {
        private readonly IConfigurationService _configurationService;

        public MysqlQueryStore(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public string GetRequestsQuery => @"SELECT
  id,
  correlation_id AS CorelationId,
  executing_stub_id AS ExecutingStubId,
  request_begin_time AS RequestBeginTime,
  request_end_time AS RequestEndTime,
  `json`
FROM requests";

        public IDbConnection GetConnection()
        {
            var config = _configurationService.GetConfiguration();
            string connectionString = config[Constants.ConfigKeys.MysqlConnectionString];
            return new MySqlConnection(connectionString);
        }
    }
}
