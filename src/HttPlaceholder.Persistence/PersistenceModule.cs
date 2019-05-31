using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Configuration;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.Implementations;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Persistence
{
    public static class PersistenceModule
    {
        public static IServiceCollection AddPersistenceModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();

            // The YAML stub source should always be registered.
            services.AddSingleton<IStubSource, YamlFileStubSource>();

            var settings = configuration.Get<SettingsModel>();
            bool registerRelationDbStubSource = false;
            string fileStoragePath = settings.Storage?.FileStorageLocation;
            string mysqlConnectionString = configuration.GetConnectionString(MysqlQueryStore.ConnectionStringKey);
            string sqliteConnectionString = configuration.GetConnectionString(SqliteQueryStore.ConnectionStringKey);
            string sqlServerConnectionString = configuration.GetConnectionString(SqlServerQueryStore.ConnectionStringKey);

            if (!string.IsNullOrWhiteSpace(fileStoragePath))
            {
                // If "fileStorageLocation" is set, it means HttPlaceholder should read and write files to a specific location.
                services.AddSingleton<IStubSource, FileSystemStubSource>();
            }
            else if (!string.IsNullOrWhiteSpace(mysqlConnectionString))
            {
                // If "mysqlConnectionString" is set, the application should connect with a MySQL database instance and store its stuff there.
                registerRelationDbStubSource = true;
                services.AddSingleton<IQueryStore, MysqlQueryStore>();
            }
            else if (!string.IsNullOrWhiteSpace(sqliteConnectionString))
            {
                // If "sqliteConnectionString" is set, the application should connect with a SQLite database instance and store its stuff there.
                registerRelationDbStubSource = true;
                services.AddSingleton<IQueryStore, SqliteQueryStore>();
            }
            else if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
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
