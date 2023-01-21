using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Web.Shared.Middleware;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Web.Shared.Tests.Middleware;

[TestClass]
public class ApiHeadersMiddlewareFacts
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

    [TestMethod]
    public async Task Invoke_RequestIsNotForApi_ShouldCallNext()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiHeadersMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/not-api/");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
    }

    [TestMethod]
    public async Task Invoke_RequestIsForApi_MethodNotOptions_ShouldCallNext()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiHeadersMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/ph-api/");

        var headers = new Dictionary<string, string> {{"Origin", "any-value"}};
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        AssertHeaders(httpContextServiceMock);
    }

    [TestMethod]
    public async Task Invoke_RequestIsForApi_MethodOptions_ShouldNotCallNext()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiHeadersMiddleware>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/ph-api/");

        var headers = new Dictionary<string, string> {{"Origin", "any-value"}};
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("OPTIONS");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsFalse(_nextCalled);
        AssertHeaders(httpContextServiceMock);
    }

    private static void AssertHeaders(Mock<IHttpContextService> mock)
    {
        mock.Verify(m => m.AddHeader("Access-Control-Allow-Origin", "*"));
        mock.Verify(m => m.AddHeader("Access-Control-Allow-Headers", "Authorization, Content-Type"));
        mock.Verify(m => m.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS"));
        mock.Verify(m => m.AddHeader("Cache-Control", "no-store, no-cache"));
        mock.Verify(m => m.AddHeader("Expires", "-1"));
    }
}
