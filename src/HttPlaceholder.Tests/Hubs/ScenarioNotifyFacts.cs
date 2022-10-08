using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Dto.v1.Scenarios;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.TestUtilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Tests.Hubs;

[TestClass]
public class ScenarioNotifyFacts
{
    private Mock<IClientProxy> _clientProxyMock;
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize()
    {
        var mocks = TestObjectFactory.CreateHubMock<ScenarioHub>();
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ScenarioSetAsync_HappyFlow()
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var notify = _mocker.CreateInstance<ScenarioNotify>();

        var input = new ScenarioStateModel();
        var mappedDto = new ScenarioStateDto();
        mapperMock
            .Setup(m => m.Map<ScenarioStateDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.ScenarioSetAsync(input, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("ScenarioSet", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task ScenarioDeletedAsync_HappyFlow()
    {
        // Arrange
        var notify = _mocker.CreateInstance<ScenarioNotify>();

        const string scenarioName = "scenario";

        // Act
        await notify.ScenarioDeletedAsync(scenarioName, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("ScenarioDeleted", It.Is<object[]>(o => (string)o.Single() == scenarioName),
            It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task AllScenariosDeletedAsync_HappyFlow()
    {
        // Arrange
        var notify = _mocker.CreateInstance<ScenarioNotify>();

        // Act
        await notify.AllScenariosDeletedAsync(CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("AllScenariosDeleted", It.IsAny<object[]>(),
            It.IsAny<CancellationToken>()));
    }
}
