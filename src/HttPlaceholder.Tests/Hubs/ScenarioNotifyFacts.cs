using System.Linq;
using AutoMapper;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.Web.Shared.Dto.v1.Scenarios;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Tests.Hubs;

[TestClass]
public class ScenarioNotifyFacts
{
    private readonly AutoMocker _mocker = new();
    private Mock<IClientProxy> _clientProxyMock;

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ScenarioSetAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        var input = new ScenarioStateModel();
        var mappedDto = new ScenarioStateDto();
        mapperMock
            .Setup(m => m.Map<ScenarioStateDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.ScenarioSetAsync(input, key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("ScenarioSet", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ScenarioDeletedAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        const string scenarioName = "scenario";

        // Act
        await notify.ScenarioDeletedAsync(scenarioName, key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("ScenarioDeleted",
            It.Is<object[]>(o => (string)o.Single() == scenarioName),
            It.IsAny<CancellationToken>()));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task AllScenariosDeletedAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        // Act
        await notify.AllScenariosDeletedAsync(key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("AllScenariosDeleted", It.IsAny<object[]>(),
            It.IsAny<CancellationToken>()));
    }

    private ScenarioNotify CreateNotify(string group)
    {
        var mocks = TestObjectFactory.CreateHubMock<ScenarioHub>(group);
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
        return _mocker.CreateInstance<ScenarioNotify>();
    }
}
