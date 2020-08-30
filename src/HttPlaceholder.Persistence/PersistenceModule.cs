using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Configuration;
using HttPlaceholder.Persistence.Db;
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
            services.AddSingleton<IStubContext, StubContext>();
            services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();

            services.AddStubSources(configuration);
            return services;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IServiceCollection AddStubSources(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.Get<SettingsModel>();
            var registerRelationDbStubSource = false;
            var fileStoragePath = settings?.Storage?.FileStorageLocation;
            var mysqlConnectionString = configuration.GetConnectionString(MysqlQueryStore.ConnectionStringKey);
            var sqliteConnectionString = configuration.GetConnectionString(SqliteQueryStore.ConnectionStringKey);
            var sqlServerConnectionString = configuration.GetConnectionString(SqlServerQueryStore.ConnectionStringKey);
            var useInMemory = settings?.Storage?.UseInMemoryStorage ?? false;
            if (useInMemory)
            {
                // User specifically wants to use the in memory stub source.
                services.AddSingleton<IStubSource, InMemoryStubSource>();
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
            else if (!string.IsNullOrWhiteSpace(fileStoragePath))
            {
                // If "fileStorageLocation" is set, it means HttPlaceholder should read and write files to a specific location.
                services.AddSingleton<IStubSource, FileSystemStubSource>();
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

            // The YAML stub source should always be registered.
            services.AddSingleton<IStubSource, YamlFileStubSource>();

            return services;
        }
    }
}
