using System.Linq;
using AutoMapper;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Tests.Hubs;

[TestClass]
public class StubNotifyFacts
{
    private readonly AutoMocker _mocker = new();
    private Mock<IClientProxy> _clientProxyMock;

    [TestInitialize]
    public void Initialize()
    {
        var mocks = TestObjectFactory.CreateHubMock<StubHub>();
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task StubAddedAsync_HappyFlow()
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var notify = _mocker.CreateInstance<StubNotify>();

        var input = new StubModel();
        var mappedDto = new StubDto();
        mapperMock
            .Setup(m => m.Map<StubDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.StubAddedAsync(input, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("StubAdded", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task StubDeletedAsync_HappyFlow()
    {
        // Arrange
        var notify = _mocker.CreateInstance<StubNotify>();

        var stubId = "stub-id";

        // Act
        await notify.StubDeletedAsync(stubId, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("StubDeleted", It.Is<object[]>(o => o.Single() == stubId),
            It.IsAny<CancellationToken>()));
    }
}
