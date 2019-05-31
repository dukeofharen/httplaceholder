using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.Implementations;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Persistence
{
    public static class PersistenceModule
    {
        public static IServiceCollection AddPersistenceModule(this IServiceCollection services)
        {
            services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();

            // The YAML stub source should always be registered.
            services.AddSingleton<IStubSource, YamlFileStubSource>();

            bool registerRelationDbStubSource = false;
            var configurationService = services.GetService<IConfigurationService>();
            var config = configurationService.GetConfiguration();
            string connectionString = null;
            if (config.TryGetValue(Constants.ConfigKeys.FileStorageLocationKey, out string fileStoragePath) && !string.IsNullOrWhiteSpace(fileStoragePath))
            {
                // If "fileStorageLocation" is set, it means HttPlaceholder should read and write files to a specific location.
                services.AddSingleton<IStubSource, FileSystemStubSource>();
            }
            else if (config.TryGetValue(Constants.ConfigKeys.MysqlConnectionStringKey, out connectionString) && !string.IsNullOrWhiteSpace(connectionString))
            {
                // If "mysqlConnectionString" is set, the application should connect with a MySQL database instance and store its stuff there.
                registerRelationDbStubSource = true;
                services.AddSingleton<IQueryStore, MysqlQueryStore>();
            }
            else if (config.TryGetValue(Constants.ConfigKeys.SqliteConnectionStringKey, out connectionString) && !string.IsNullOrWhiteSpace(connectionString))
            {
                // If "sqliteConnectionString" is set, the application should connect with a SQLite database instance and store its stuff there.
                registerRelationDbStubSource = true;
                services.AddSingleton<IQueryStore, SqliteQueryStore>();
            }
            else if (config.TryGetValue(Constants.ConfigKeys.SqlServerConnectionStringKey, out connectionString) && !string.IsNullOrWhiteSpace(connectionString))
            {
                // If "sqlServerConnectionString" is set, the application should connect with a MS SQL Server database instance and store its stuff there.
                registerRelationDbStubSource = true;
                services.AddSingleton<IQueryStore, SqlServerQueryStore>();
            }
            else
            {
                // If no suitable configuration is found, store all stubs in memory by default.
                services.AddSingleton<IStubSource, InMemoryStubSource>();
            }

            if (registerRelationDbStubSource)
            {
                services.AddSingleton<IStubSource, RelationalDbStubSource>();
            }

            return services;
        }
    }
}
