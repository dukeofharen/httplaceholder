using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.FileSystem.Implementations;
using HttPlaceholder.Persistence.StubSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Persistence.Tests;

[TestClass]
public class PersistenceModuleFacts
{
    private readonly Dictionary<string, string> _args = new();
    private readonly IServiceCollection _services = new ServiceCollection();

    [TestMethod]
    public void
        DependencyRegistration_AddStubSources_InputFileKeySet_ShouldRegisterYamlFileStubSourceAndInMemoryStubSource()
    {
        // arrange
        _args.Add("Storage:InputFile", @"C:\yamlFiles");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(2, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(InMemoryStubSource)));
    }

    [TestMethod]
    public void
        DependencyRegistration_AddStubSources_UseInMemoryStubSource()
    {
        // arrange
        _args.Add("Storage:UseInMemoryStorage", @"true");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(2, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(InMemoryStubSource)));
    }

    [TestMethod]
    public void DependencyRegistration_AddStubSources_FileStorageLocationKeySet_ShouldRegisterFileSystemStubSource()
    {
        // arrange
        _args.Add("Storage:FileStorageLocation", @"C:\storage");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(3, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileSystemStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileSystemStubCache)));
    }

    [TestMethod]
    public void DependencyRegistration_AddStubSources_MysqlConnectionStringKeySet_ShouldRegisterStubSource()
    {
        // arrange
        _args.Add("ConnectionStrings:MySql",
            "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(7, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(MysqlQueryStore)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(MysqlDbConnectionFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(DatabaseContextFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubCache)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbMigrator)));
    }

    [TestMethod]
    public void DependencyRegistration_AddStubSources_SqliteConnectionStringKeySet_ShouldRegisterStubSource()
    {
        // arrange
        _args.Add("ConnectionStrings:Sqlite", "Data Source=app.db");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(7, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqliteQueryStore)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqliteDbConnectionFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(DatabaseContextFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubCache)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbMigrator)));
    }

    [TestMethod]
    public void DependencyRegistration_AddStubSources_SqlServerConnectionStringKeySet_ShouldRegisterStubSource()
    {
        // arrange
        _args.Add("ConnectionStrings:SqlServer",
            "Server=localhost;Database=httplaceholder;User Id=sa;Password=Password123");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(7, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqlServerQueryStore)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqlServerDbConnectionFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(DatabaseContextFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubCache)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbMigrator)));
    }

    [TestMethod]
    public void DependencyRegistration_AddStubSources_PostgresConnectionStringKeySet_ShouldRegisterStubSource()
    {
        // arrange
        _args.Add("ConnectionStrings:Postgres",
            "Host=localhost,5432;Username=postgres;Password=postgres;Database=httplaceholder;SearchPath=public");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.AreEqual(7, _services.Count);
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(PostgresQueryStore)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(PostgresDbConnectionFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(DatabaseContextFactory)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubCache)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbMigrator)));
    }

    [TestMethod]
    public void DependencyRegistration_DisableFileWatcher()
    {
        // arrange
        _args.Add("Storage:DisableFileWatcher", "true");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
        Assert.IsFalse(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
    }

    [TestMethod]
    public void DependencyRegistration_EnableFileWatcher()
    {
        // arrange
        _args.Add("Storage:DisableFileWatcher", "false");

        // act
        _services.AddStubSources(BuildConfiguration(_args));

        // assert
        Assert.IsFalse(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
        Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileWatcherYamlFileStubSource)));
    }

    private static IConfiguration BuildConfiguration(IDictionary<string, string> dict) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
}
