using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.FileSystem;
using HttPlaceholder.Persistence.FileSystem.Implementations;
using HttPlaceholder.Persistence.Implementations;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Persistence;

public static class PersistenceModule
{
    public static IServiceCollection AddPersistenceModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IStubContext, StubContext>();
        services.AddSingleton<IStubRootPathResolver, StubRootPathResolver>();
        services.AddSingleton<IScenarioStateStore, ScenarioStateStore>();

        services.AddStubSources(configuration);
        return services;
    }

    public static IServiceCollection AddStubSources(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.Get<SettingsModel>();
        var registerRelationDbStubSource = false;
        var fileStoragePath = settings?.Storage?.FileStorageLocation;
        var mysqlConnectionString = configuration.GetConnectionString(MysqlDbConnectionFactory.ConnectionStringKey);
        var sqliteConnectionString = configuration.GetConnectionString(SqliteDbConnectionFactory.ConnectionStringKey);
        var sqlServerConnectionString = configuration.GetConnectionString(SqlServerDbConnectionFactory.ConnectionStringKey);
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
            services.AddSingleton<IDbConnectionFactory, MysqlDbConnectionFactory>();
        }
        else if (!string.IsNullOrWhiteSpace(sqliteConnectionString))
        {
            // If "sqliteConnectionString" is set, the application should connect with a SQLite database instance and store its stuff there.
            registerRelationDbStubSource = true;
            services.AddSingleton<IQueryStore, SqliteQueryStore>();
            services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();
        }
        else if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
        {
            // If "sqlServerConnectionString" is set, the application should connect with a MS SQL Server database instance and store its stuff there.
            registerRelationDbStubSource = true;
            services.AddSingleton<IQueryStore, SqlServerQueryStore>();
            services.AddSingleton<IDbConnectionFactory, SqlServerDbConnectionFactory>();
        }
        else if (!string.IsNullOrWhiteSpace(fileStoragePath))
        {
            // If "fileStorageLocation" is set, it means HttPlaceholder should read and write files to a specific location.
            services.AddSingleton<IStubSource, FileSystemStubSource>();
            services.AddSingleton<IFileSystemStubCache, FileSystemStubCache>();
        }
        else
        {
            // If no suitable configuration is found, store all stubs in memory by default.
            services.AddSingleton<IStubSource, InMemoryStubSource>();
        }

        if (registerRelationDbStubSource)
        {
            services.AddSingleton<IStubSource, RelationalDbStubSource>();
            services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
            services.AddSingleton<IRelationalDbStubCache, RelationalDbStubCache>();
        }

        // The YAML stub source should always be registered.
        services.AddSingleton<IStubSource, YamlFileStubSource>();

        return services;
    }
}