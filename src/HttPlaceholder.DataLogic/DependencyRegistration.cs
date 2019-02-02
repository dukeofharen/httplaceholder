using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.DataLogic.Db.Implementations;
using HttPlaceholder.DataLogic.Implementations;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.DataLogic
{
    public static class DependencyRegistration
    {
        public static IServiceCollection AddDataLogic(this IServiceCollection services)
        {
            services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();
            return services;
        }

        public static IServiceCollection AddStubSources(this IServiceCollection services)
        {
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