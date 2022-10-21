using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Tests.Middleware;

[TestClass]
public class ApiExceptionHandlingMiddlewareFacts
{
    private readonly AutoMocker _mocker = new();
    private Exception _exceptionToThrow;
    private bool _nextCalled;

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use<RequestDelegate>(_ =>
        {
            _nextCalled = true;
            if (_exceptionToThrow != null)
            {
                throw _exceptionToThrow;
            }

            return Task.CompletedTask;
        });

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Invoke_RequestPathNotApi_ShouldCallNext()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/not-api");

        _exceptionToThrow = new ArgumentException("Should not be caught.");

        // Act
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => middleware.Invoke(null));

        // Assert
        Assert.IsTrue(_nextCalled);
    }

    [TestMethod]
    public async Task Invoke_PathIsApi_ConflictException_ShouldReturn409()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/ph-api/");
        _exceptionToThrow = new ConflictException("conflict");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        mockHttpContextService.Verify(m => m.SetStatusCode(HttpStatusCode.Conflict));
    }

    [TestMethod]
    public async Task Invoke_PathIsApi_NotFoundException_ShouldReturn404()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/ph-api/");
        _exceptionToThrow = new NotFoundException("not found");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        mockHttpContextService.Verify(m => m.SetStatusCode(HttpStatusCode.NotFound));
    }

    [TestMethod]
    public async Task Invoke_PathIsApi_ForbiddenException_ShouldReturn403()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/ph-api/");
        _exceptionToThrow = new ForbiddenException();

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        mockHttpContextService.Verify(m => m.SetStatusCode(HttpStatusCode.Forbidden));
    }

    [TestMethod]
    public async Task Invoke_PathIsApi_ArgumentException_ShouldReturn400AndResponse()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/ph-api/");

        string capturedBody = null;
        mockHttpContextService
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((b, _) => capturedBody = b)
            .Returns(Task.CompletedTask);
        _exceptionToThrow = new ArgumentException("ERROR!");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        mockHttpContextService.Verify(m => m.SetStatusCode(HttpStatusCode.BadRequest));

        Assert.IsNotNull(capturedBody);
        var jArray = JArray.Parse(capturedBody);
        Assert.AreEqual("ERROR!", jArray[0].ToString());
    }

    [TestMethod]
    public async Task Invoke_PathIsApi_ValidationException_ShouldReturn400AndResponse()
    {
        // Arrange
        var middleware = _mocker.CreateInstance<ApiExceptionHandlingMiddleware>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        mockHttpContextService
            .Setup(m => m.Path)
            .Returns("/ph-api/");

        string capturedBody = null;
        mockHttpContextService
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((b, _) => capturedBody = b)
            .Returns(Task.CompletedTask);
        _exceptionToThrow = new ValidationException("ERROR!");

        // Act
        await middleware.Invoke(null);

        // Assert
        Assert.IsTrue(_nextCalled);
        mockHttpContextService.Verify(m => m.SetStatusCode(HttpStatusCode.BadRequest));

        Assert.IsNotNull(capturedBody);
        var jArray = JArray.Parse(capturedBody);
        Assert.AreEqual("ERROR!", jArray[0].ToString());
    }
}
