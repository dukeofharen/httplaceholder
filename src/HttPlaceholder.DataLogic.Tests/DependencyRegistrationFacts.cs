using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.DataLogic.Db.Implementations;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.DataLogic.Tests
{
    [TestClass]
    public class DependencyRegistrationFacts
    {
        private IServiceCollection _services;
        private IDictionary<string, string> _args;
        private Mock<IConfigurationService> _configurationServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            _services = new ServiceCollection();
            _args = new Dictionary<string, string>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _services.AddTransient(provider => _configurationServiceMock.Object);
            _configurationServiceMock
               .Setup(m => m.GetConfiguration())
               .Returns(_args);
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_InputFileKeySet_ShouldRegisterYamlFileStubSourceAndInMemoryStubSource()
        {
            // arrange
            _args.Add(Constants.ConfigKeys.InputFileKey, @"C:\yamlFiles");

            // act
            _services.AddStubSources();

            // assert
            Assert.AreEqual(3, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(InMemoryStubSource)));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IConfigurationService)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_FileStorageLocationKeySet_ShouldRegisterFileSystemStubSource()
        {
            // arrange
            _args.Add(Constants.ConfigKeys.FileStorageLocation, @"C:\storage");

            // act
            _services.AddStubSources();

            // assert
            Assert.AreEqual(3, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(FileSystemStubSource)));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IConfigurationService)));
        }

        [TestMethod]
        public void DependencyRegistration_AddStubSources_MysqlConnectionStringKeySet_ShouldRegisterFileSystemStubSource()
        {
            // arrange
            _args.Add(Constants.ConfigKeys.MysqlConnectionString, "Server=localhost;Database=httplaceholder;Uid=httplaceholder;Pwd=httplaceholder");

            // act
            _services.AddStubSources();

            // assert
            Assert.AreEqual(4, _services.Count);
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(YamlFileStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(RelationalDbStubSource)));
            Assert.IsTrue(_services.Any(s => s.ImplementationType == typeof(MysqlQueryStore)));
            Assert.IsTrue(_services.Any(s => s.ServiceType == typeof(IConfigurationService)));
        }
    }
}