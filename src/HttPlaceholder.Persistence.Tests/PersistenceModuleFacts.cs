using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Configuration;
using HttPlaceholder.Configuration.Utilities;
using HttPlaceholder.Persistence.Db.Implementations;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Persistence.Tests
{
    [TestClass]
    public class PersistenceModuleFacts
    {
        private IServiceCollection _services;
        private IDictionary<string, string> _args;

        [TestInitialize]
        public void Initialize()
        {
            _services = new ServiceCollection();
            _args = new Dictionary<string, string>();
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_InputFileKeySet_ShouldRegisterYamlFileStubSourceAndInMemoryStubSource()
        {
            // arrange
            _args.Add(ConfigKeys.InputFileKey, @"C:\yamlFiles");

            // act
            _services.AddStubSources(BuildConfiguration(_args));

            // assert
            Assert.AreEqual(2, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(InMemoryStubSource)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_FileStorageLocationKeySet_ShouldRegisterFileSystemStubSource()
        {
            // arrange
            _args.Add(ConfigKeys.FileStorageLocationKey, @"C:\storage");

            // act
            _services.AddStubSources(BuildConfiguration(_args));

            // assert
            Assert.AreEqual(2, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileSystemStubSource)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_MysqlConnectionStringKeySet_ShouldRegisterStubSource()
        {
            // arrange
            _args.Add(ConfigKeys.MysqlConnectionStringKey, "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder");

            // act
            _services.AddStubSources(BuildConfiguration(_args));

            // assert
            Assert.AreEqual(3, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(MysqlQueryStore)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_SqliteConnectionStringKeySet_ShouldRegisterStubSource()
        {
            // arrange
            _args.Add(ConfigKeys.SqliteConnectionStringKey, "Data Source=app.db");

            // act
            _services.AddStubSources(BuildConfiguration(_args));

            // assert
            Assert.AreEqual(3, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqliteQueryStore)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_SqlServerConnectionStringKeySet_ShouldRegisterStubSource()
        {
            // arrange
            _args.Add(ConfigKeys.SqlServerConnectionStringKey, "Server=localhost;Database=httplaceholder;User Id=sa;Password=Password123");

            // act
            _services.AddStubSources(BuildConfiguration(_args));

            // assert
            Assert.AreEqual(3, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(SqlServerQueryStore)));
        }

        private IConfiguration BuildConfiguration(IDictionary<string, string> argsDictionary) =>
            new ConfigurationBuilder()
                .AddHttPlaceholderConfiguration(argsDictionary)
                .Build();
    }
}
