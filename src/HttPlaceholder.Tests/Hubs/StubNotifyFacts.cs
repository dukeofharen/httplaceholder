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

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task StubAddedAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        var input = new FullStubOverviewModel();
        var mappedDto = new FullStubOverviewDto();
        mapperMock
            .Setup(m => m.Map<FullStubOverviewDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.StubAddedAsync(input, key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("StubAdded", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task StubDeletedAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        const string stubId = "stub-id";

        // Act
        await notify.StubDeletedAsync(stubId, key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("StubDeleted", It.Is<object[]>(o => (string)o.Single() == stubId),
            It.IsAny<CancellationToken>()));
    }

    private StubNotify CreateNotify(string group)
    {
        var mocks = TestObjectFactory.CreateHubMock<StubHub>(group);
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
        return _mocker.CreateInstance<StubNotify>();
    }
}
