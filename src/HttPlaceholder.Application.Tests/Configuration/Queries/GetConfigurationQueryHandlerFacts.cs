using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Queries.GetConfiguration;
using HttPlaceholder.Application.Interfaces.Configuration;
using HttPlaceholder.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Configuration.Queries;

[TestClass]
public class GetConfigurationQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var configHelperMock = _mocker.GetMock<IConfigurationHelper>();

        var configMetadata = new[]
        {
            new ConfigMetadataModel
            {
                Key = "key1",
                Path = "Path:Key1",
                ConfigKeyType = ConfigKeyType.Authentication,
                Description = "Description1",
                IsSecretValue = false
            },
            new ConfigMetadataModel
            {
                Key = "key2",
                Path = "Path:Key2",
                ConfigKeyType = ConfigKeyType.Configuration,
                Description = "Description2",
                IsSecretValue = true
            },
            new ConfigMetadataModel
            {
                Key = "key3",
                Path = "Path:Key3",
                ConfigKeyType = ConfigKeyType.Stub,
                Description = "Description3",
                IsSecretValue = false
            }
        };

        configHelperMock
            .Setup(m => m.GetConfigKeyMetadata())
            .Returns(configMetadata);

        var testConfig = new Dictionary<string, string> {{"Path:Key1", "value1"}, {"Path:Key2", "value2"}};
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(testConfig)
            .Build();
        _mocker.Use<IConfiguration>(configuration);

        var handler = _mocker.CreateInstance<GetConfigurationQueryHandler>();

        // Act
        var result = (await handler.Handle(new GetConfigurationQuery(), CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        var item1 = result[0];
        Assert.AreEqual("key1", item1.Key);
        Assert.AreEqual("Path:Key1", item1.Path);
        Assert.AreEqual(ConfigKeyType.Authentication, item1.ConfigKeyType);
        Assert.AreEqual("Description1", item1.Description);
        Assert.AreEqual("value1", item1.Value);

        var item2 = result[1];
        Assert.AreEqual("key2", item2.Key);
        Assert.AreEqual("Path:Key2", item2.Path);
        Assert.AreEqual(ConfigKeyType.Configuration, item2.ConfigKeyType);
        Assert.AreEqual("Description2", item2.Description);
        Assert.AreEqual("***", item2.Value);
    }
}
