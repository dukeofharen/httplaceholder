using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Web.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Web.Shared.Tests.Middleware;

[TestClass]
public class StubHandlingMiddlewareFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<StubHandlingMiddleware> _mockLogger = new();
    private readonly Mock<IRequestLoggerFactory> _requestLoggerFactoryMock = new();
    private readonly Mock<IRequestLogger> _requestLoggerMock = new();

    private readonly SettingsModel _settings = new()
    {
        Stub = new StubSettingsModel(), Storage = new StorageSettingsModel(), Gui = new GuiSettingsModel()
    };

    private bool _nextCalled;

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<ILogger<StubHandlingMiddleware>>(_mockLogger);
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
        _mocker.Use<RequestDelegate>(_ =>
        {
            _nextCalled = true;
            return Task.CompletedTask;
        });
        _mocker.Use(_requestLoggerFactoryMock.Object);

        _requestLoggerFactoryMock
            .Setup(m => m.GetRequestLogger())
            .Returns(_requestLoggerMock.Object);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Invoke_HealthCheckOnRootUrl_ShouldReturnDirectly()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        _settings.Stub.HealthcheckOnRootUrl = true;
        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.WriteAsync("OK", It.IsAny<CancellationToken>()));
    }

    [DataTestMethod]
    [DataRow("/ph-api")]
    [DataRow("/ph-ui")]
    [DataRow("/ph-static")]
    [DataRow("/swagger")]
    [DataRow("/requestHub")]
    [DataRow("/scenarioHub")]
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
        _mocker.GetMock<IStubHandler>()
            .Verify(m => m.HandleStubRequestAsync(It.IsAny<CancellationToken>()));
    }
}
