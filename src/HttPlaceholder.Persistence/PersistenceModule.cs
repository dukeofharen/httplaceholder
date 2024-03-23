using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.Db;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.FileSystem;
using HttPlaceholder.Persistence.FileSystem.Implementations;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Persistence;

/// <summary>
///     A class that is used to register all classes in the Persistence module on the service collection.
/// </summary>
public static class PersistenceModule
{
    /// <summary>
    ///     Register all classes in the Persistence module on the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddPersistenceModule(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .Scan(scan => scan.FromCallingAssembly().RegisterDependencies())
            .AddStubSources(configuration);

    /// <summary>
    ///     Register specific stub sources on the service collection based on the configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static IServiceCollection AddStubSources(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.Get<SettingsModel>();
        if (settings?.Storage?.UseInMemoryStorage ?? false)
        {
            // User specifically wants to use the in memory stub source.
            services.AddSingleton<IStubSource, InMemoryStubSource>();
        }
        else if (!string.IsNullOrWhiteSpace(
                     configuration.GetConnectionString(MysqlDbConnectionFactory.ConnectionStringKey)))
        {
            // If "mysqlConnectionString" is set, the application should connect with a MySQL database instance and store its stuff there.
            services.RegisterConnectionFactory<MysqlDbConnectionFactory, MysqlQueryStore>();
        }
        else if (!string.IsNullOrWhiteSpace(
                     configuration.GetConnectionString(SqliteDbConnectionFactory.ConnectionStringKey)))
        {
            // If "sqliteConnectionString" is set, the application should connect with a SQLite database instance and store its stuff there.
            services.RegisterConnectionFactory<SqliteDbConnectionFactory, SqliteQueryStore>();
        }
        else if (!string.IsNullOrWhiteSpace(
                     configuration.GetConnectionString(SqlServerDbConnectionFactory.ConnectionStringKey)))
        {
            // If "sqlServerConnectionString" is set, the application should connect with a MS SQL Server database instance and store its stuff there.
            services.RegisterConnectionFactory<SqlServerDbConnectionFactory, SqlServerQueryStore>();
        }
        else if (!string.IsNullOrWhiteSpace(
                     configuration.GetConnectionString(PostgresDbConnectionFactory.ConnectionStringKey)))
        {
            // If "postgresConnectionString" is set, the application should connect with a MS SQL Server database instance and store its stuff there.
            services.RegisterConnectionFactory<PostgresDbConnectionFactory, PostgresQueryStore>();
        }
        else if (!string.IsNullOrWhiteSpace(settings?.Storage?.FileStorageLocation))
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

        // The YAML stub source should always be registered. If stated that the file watcher should be disabled, register the YamlFileStubSource instead.
        var disableFileWatcher = settings?.Storage?.DisableFileWatcher ?? false;
        if (disableFileWatcher)
        {
            services.AddSingleton<IStubSource, YamlFileStubSource>();
        }
        else
        {
            services.AddSingleton<IStubSource, FileWatcherYamlFileStubSource>();
        }

        return services;
    }

    private static IServiceCollection RegisterConnectionFactory<TConnectionFactory, TQueryStore>(
        this IServiceCollection services)
        where TConnectionFactory : class, IDbConnectionFactory where TQueryStore : class, IQueryStore =>
        services
            .AddSingleton<IQueryStore, TQueryStore>()
            .AddSingleton<IDbConnectionFactory, TConnectionFactory>()
            .AddSingleton<IStubSource, RelationalDbStubSource>()
            .AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>()
            .AddSingleton<IRelationalDbStubCache, RelationalDbStubCache>()
            .AddSingleton<IRelationalDbMigrator, RelationalDbMigrator>();
}
