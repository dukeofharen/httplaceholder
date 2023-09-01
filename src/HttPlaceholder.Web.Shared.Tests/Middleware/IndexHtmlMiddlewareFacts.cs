﻿using HtmlAgilityPack;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Web.Shared.Middleware;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Web.Shared.Tests.Middleware;

[TestClass]
public class IndexHtmlMiddlewareFacts
{
    private const string UiPath = "/var/httplaceholder/ui";
    private readonly AutoMocker _mocker = new();
    private bool _nextCalled;

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<RequestDelegate>(_ =>
        {
            _nextCalled = true;
            return Task.CompletedTask;
        });
        _mocker.Use(UiPath);
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Invoke_PathNotForUi_ShouldContinue()
    {
        // Arrange
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var middleware = _mocker.CreateInstance<IndexHtmlMiddleware>();

        const string path = "/not-ui";
        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

        // Act
        await middleware.Invoke(new MockHttpContext());

        // Assert
        Assert.IsTrue(_nextCalled);
        httpContextServiceMock
            .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow("/ph-ui")]
    [DataRow("/ph-ui/")]
    [DataRow("/ph-ui/index.html")]
    public async Task Invoke_PathForUi_ShouldReturnModifiedIndexHtml(string path)
    {
        // Arrange
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var htmlServiceMock = _mocker.GetMock<IHtmlService>();
        var middleware = _mocker.CreateInstance<IndexHtmlMiddleware>();
        IndexHtmlMiddleware.IndexHtml = null;

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

        const string indexHtml = "<html><head></head><body></body></html>";
        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(Path.Join(UiPath, "index.html"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(indexHtml);

        const string rootUrl = "http://localhost/httplaceholder";
        urlResolverMock
            .Setup(m => m.GetRootUrl())
            .Returns(rootUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(indexHtml);
        htmlServiceMock
            .Setup(m => m.ReadHtml(indexHtml))
            .Returns(doc);

        string capturedHtml = null;
        httpContextServiceMock
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((h, _) => capturedHtml = h);

        // Act
        await middleware.Invoke(new MockHttpContext());

        // Assert
        Assert.IsFalse(_nextCalled);
        Assert.IsNotNull(capturedHtml);
        Assert.AreEqual(
            """<html><head><base href="http://localhost/httplaceholder"><script type="text/javascript">window.rootUrl = "http://localhost/httplaceholder";</script></head><body></body></html>""",
            capturedHtml);
        httpContextServiceMock.Verify(m => m.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime));
    }

    [TestMethod]
    public async Task Invoke_PathForUi_ShouldCache()
    {
        // Arrange
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var htmlServiceMock = _mocker.GetMock<IHtmlService>();
        var middleware = _mocker.CreateInstance<IndexHtmlMiddleware>();
        IndexHtmlMiddleware.IndexHtml = null;

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns("/ph-ui/index.html");

        const string indexHtml = "<html><head></head><body></body></html>";
        fileServiceMock
            .Setup(m => m.ReadAllTextAsync(Path.Join(UiPath, "index.html"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(indexHtml);

        const string rootUrl = "http://localhost/httplaceholder";
        urlResolverMock
            .Setup(m => m.GetRootUrl())
            .Returns(rootUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(indexHtml);
        htmlServiceMock
            .Setup(m => m.ReadHtml(indexHtml))
            .Returns(doc);

        string capturedHtml = null;
        httpContextServiceMock
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((h, _) => capturedHtml = h);

        // Act
        await middleware.Invoke(new MockHttpContext());
        await middleware.Invoke(new MockHttpContext());
        await middleware.Invoke(new MockHttpContext());

        // Assert
        Assert.IsFalse(_nextCalled);
        Assert.IsNotNull(capturedHtml);
        urlResolverMock.Verify(m => m.GetRootUrl(), Times.Once);
    }
}