using System.Net;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Web.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

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
    public async Task Invoke_ExecuteStub_HappyFlow()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        const string method = "POST";
        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns(method);

        httpContextServiceMock
            .Setup(m => m.DisplayUrl)
            .Returns(requestPath);

        var requestBody = "posted body"u8.ToArray();
        httpContextServiceMock
            .Setup(m => m.GetBodyAsBytesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestBody);

        const string ip = "1.2.3.4";
        clientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        var requestHeaders = new Dictionary<string, string>();
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(requestHeaders);

        var stubResponse = new ResponseModel
        {
            Body = new byte[] {1, 2, 3},
            Headers = {{"X-Header1", "val1"}, {"X-Header2", "val2"}},
            StatusCode = 201,
            BodyIsBinary = true
        };
        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResponse);

        var requestResultModel = new RequestResultModel {ExecutingStubId = "stub123"};
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.EnableRewind());
        httpContextServiceMock.Verify(m => m.ClearResponse());
        httpContextServiceMock.Verify(m =>
            m.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, It.IsAny<StringValues>()));
        httpContextServiceMock.Verify(m =>
            m.TryAddHeader(HeaderKeys.XHttPlaceholderExecutedStub, requestResultModel.ExecutingStubId));
        httpContextServiceMock.Verify(m => m.SetStatusCode(stubResponse.StatusCode));
        httpContextServiceMock.Verify(m => m.AddHeader("X-Header1", "val1"));
        httpContextServiceMock.Verify(m => m.AddHeader("X-Header2", "val2"));
        httpContextServiceMock.Verify(m => m.WriteAsync(stubResponse.Body, It.IsAny<CancellationToken>()));
        stubContextMock.Verify(m =>
            m.AddRequestResultAsync(requestResultModel, stubResponse, It.IsAny<CancellationToken>()));
        Assert.IsFalse(_mockLogger.Contains(LogLevel.Information, "Request: "));
    }

    [TestMethod]
    public async Task Invoke_ExecuteStub_HappyFlow_AbortConnection()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/path");

        var stubResponse = new ResponseModel {AbortConnection = true};
        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResponse);

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.AbortConnection());
    }

    [TestMethod]
    public async Task Invoke_ExecuteStub_HappyFlow_NoRequestBody_ShouldNotWriteRequestBody()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        var stubResponse = new ResponseModel();
        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResponse);

        httpContextServiceMock
            .Setup(m => m.GetBodyAsBytesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[])null);

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }

    [TestMethod]
    public async Task Invoke_ExecuteStub_HappyFlow_RequestLoggingEnabled_ShouldLogRequest()
    {
        // Arrange
        _settings.Storage.EnableRequestLogging = true;
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        var stubResponse = new ResponseModel();
        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResponse);

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Information, "Request: "));
    }

    [TestMethod]
    public async Task
        Invoke_ExecuteStub_UiEnabled_RequestValidationExceptionWhenExecutingStub_ShouldReturn501WithHtmlPage()
    {
        // Arrange
        _settings.Storage.EnableRequestLogging = true;
        _settings.Gui.EnableUserInterface = true;
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();
        var resourcesServiceMock = _mocker.GetMock<IResourcesService>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestValidationException("ERROR!"));

        const string page501 = "Not implemented [ROOT_URL]";
        resourcesServiceMock
            .Setup(m => m.ReadAsString("Files/StubNotConfigured.html"))
            .Returns(page501);

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.SetStatusCode(HttpStatusCode.NotImplemented));
        httpContextServiceMock.Verify(m =>
            m.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, It.IsAny<StringValues>()));
        httpContextServiceMock.Verify(m => m.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime));
        httpContextServiceMock.Verify(m =>
            m.WriteAsync(It.Is<string>(b => b.Contains("Not implemented")), It.IsAny<CancellationToken>()));
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, "Request validation exception thrown:"));
    }

    [TestMethod]
    public async Task
        Invoke_ExecuteStub_UiEnabled_TaskCanceledExceptionWhenExecutingStub_ShouldReturn501WithHtmlPage()
    {
        // Arrange
        _settings.Storage.EnableRequestLogging = true;
        _settings.Gui.EnableUserInterface = true;
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException("Cancelled"));

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, "Request was cancelled."));
    }

    [TestMethod]
    public async Task
        Invoke_ExecuteStub_UiDisabled_RequestValidationExceptionWhenExecutingStub_ShouldReturn501WithoutHtmlPage()
    {
        // Arrange
        _settings.Storage.EnableRequestLogging = true;
        _settings.Gui.EnableUserInterface = false;
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestValidationException("ERROR!"));

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.SetStatusCode(HttpStatusCode.NotImplemented));
        httpContextServiceMock.Verify(m =>
            m.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, It.IsAny<StringValues>()));
        httpContextServiceMock.Verify(m => m.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime), Times.Never);
        httpContextServiceMock.Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Debug, "Request validation exception thrown:"));
    }

    [TestMethod]
    public async Task Invoke_ExecuteStub_AnotherExceptionWhileExecutingStub_ShouldReturn500()
    {
        // Arrange
        _settings.Storage.EnableRequestLogging = true;
        var middleware = _mocker.CreateInstance<StubHandlingMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stubRequestExecutorMock = _mocker.GetMock<IStubRequestExecutor>();

        const string requestPath = "/stub-path";

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(requestPath);

        stubRequestExecutorMock
            .Setup(m => m.ExecuteRequestAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("ERROR!"));

        var requestResultModel = new RequestResultModel();
        _requestLoggerMock
            .Setup(m => m.GetResult())
            .Returns(requestResultModel);

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        httpContextServiceMock.Verify(m => m.SetStatusCode(HttpStatusCode.InternalServerError));
        httpContextServiceMock.Verify(m =>
            m.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, It.IsAny<StringValues>()));
        httpContextServiceMock.Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.IsTrue(_mockLogger.Contains(LogLevel.Warning, "Unexpected exception thrown:"));
    }
}
