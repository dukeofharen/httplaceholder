using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Queries.GetConfiguration;
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
        var testConfig = new Dictionary<string, string> {{"Authentication:ApiUsername", "value1"}, {"Authentication:ApiPassword", "value2"}};
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
        Assert.AreEqual("apiUsername", item1.Key);
        Assert.AreEqual("Authentication:ApiUsername", item1.Path);
        Assert.AreEqual(ConfigKeyType.Authentication, item1.ConfigKeyType);
        Assert.AreEqual("value1", item1.Value);

        var item2 = result[1];
        Assert.AreEqual("apiPassword", item2.Key);
        Assert.AreEqual("Authentication:ApiPassword", item2.Path);
        Assert.AreEqual(ConfigKeyType.Authentication, item2.ConfigKeyType);
        Assert.AreEqual("***", item2.Value);
    }
}
