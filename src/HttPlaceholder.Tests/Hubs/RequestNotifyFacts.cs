using System.Linq;
using AutoMapper;
using HttPlaceholder.Hubs.Implementations;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using Microsoft.AspNetCore.SignalR;

namespace HttPlaceholder.Tests.Hubs;

[TestClass]
public class RequestNotifyFacts
{
    private readonly AutoMocker _mocker = new();
    private Mock<IClientProxy> _clientProxyMock;

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task NewRequestReceivedAsync_HappyFlow(bool withDistributionKey)
    {
        // Arrange
        var mapperMock = _mocker.GetMock<IMapper>();
        var key = withDistributionKey ? "dist-key" : null;
        var notify = CreateNotify(key);

        var input = new RequestResultModel();
        var mappedDto = new RequestOverviewDto();
        mapperMock
            .Setup(m => m.Map<RequestOverviewDto>(input))
            .Returns(mappedDto);

        // Act
        await notify.NewRequestReceivedAsync(input, key, CancellationToken.None);

        // Assert
        _clientProxyMock.Verify(m => m.SendCoreAsync("RequestReceived", It.Is<object[]>(o => o.Single() == mappedDto),
            It.IsAny<CancellationToken>()));
    }

    private RequestNotify CreateNotify(string group)
    {
        var mocks = TestObjectFactory.CreateHubMock<RequestHub>(group);
        _mocker.Use(mocks.hubContext);
        _clientProxyMock = mocks.clientProxyMock;
        return _mocker.CreateInstance<RequestNotify>();
    }
}
