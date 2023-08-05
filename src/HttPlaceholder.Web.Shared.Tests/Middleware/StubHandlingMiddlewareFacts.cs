using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Commands.HandleStubRequest;
using HttPlaceholder.Web.Shared.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Web.Shared.Tests.Middleware;

[TestClass]
public class StubHandlingMiddlewareFacts
{
    private readonly AutoMocker _mocker = new();

    private bool _nextCalled;

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use<RequestDelegate>(_ =>
        {
            _nextCalled = true;
            return Task.CompletedTask;
        });

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow("/ph-api")]
    [DataRow("/ph-ui")]
    [DataRow("/ph-static")]
    [DataRow("/swagger")]
    [DataRow("/requestHub")]
    [DataRow("/scenarioHub")]
    [DataRow("/stubHub")]
    public async Task Invoke_SegmentToBeIgnored_ShouldCallNextDirectly(string segment)
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(segment);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
    }

    [TestMethod]
    public async Task Invoke_StubShouldBeExecuted()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        const string requestPath = "/stub-path";
        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        // Act
        await middleware.Invoke(null);

        // Assert
        _mocker.GetMock<IMediator>()
            .Verify(m => m.Send(It.IsAny<HandleStubRequestCommand>(), It.IsAny<CancellationToken>()));
    }
}
