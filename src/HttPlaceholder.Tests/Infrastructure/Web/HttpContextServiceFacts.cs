using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Infrastructure.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Tests.Infrastructure.Web;

[TestClass]
public class HttpContextServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockHttpContext _mockHttpContext = new();

    [TestInitialize]
    public void Initialize()
    {
        var httpContextAccessorMock = _mocker.GetMock<IHttpContextAccessor>();
        httpContextAccessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockHttpContext);
    }

    [TestMethod]
    public void Method_HappyFlow()
    {
        // Arrange
        _mockHttpContext.SetRequestMethod("POST");
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.Method;

        // Assert
        Assert.AreEqual("POST", result);
    }

    [TestMethod]
    public void Path_HappyFlow()
    {
        // Arrange
        _mockHttpContext.SetRequestPath("/path");
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.Path;

        // Assert
        Assert.AreEqual("/path", result);
    }

    [TestMethod]
    public void FullPath_HappyFlow()
    {
        // Arrange
        _mockHttpContext.SetRequestPath("/path");
        _mockHttpContext.SetQueryString("?key1=val1");
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.FullPath;

        // Assert
        Assert.AreEqual("/path?key1=val1", result);
    }

    [DataTestMethod]
    [DataRow(false, "httplaceholder.com", "/path", "?key1=val1", "http://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", "?key1=val1", "https://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", null, "https://httplaceholder.com/path")]
    public void DisplayUrl_HappyFlow(bool isHttps, string host, string path, string queryString, string expectedUrl)
    {
        // Arrange
        _mockHttpContext.SetRequestPath(path);
        _mockHttpContext.SetQueryString(queryString);

        var mockClientDataResolver = _mocker.GetMock<IClientDataResolver>();
        mockClientDataResolver
            .Setup(m => m.IsHttps())
            .Returns(isHttps);
        mockClientDataResolver
            .Setup(m => m.GetHost())
            .Returns(host);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.DisplayUrl;

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }

    [DataTestMethod]
    [DataRow(false, "httplaceholder.com", "http://httplaceholder.com")]
    [DataRow(true, "httplaceholder.com", "https://httplaceholder.com")]
    public void RootUrl_HappyFlow(bool isHttps, string host, string expectedUrl)
    {
        // Arrange
        var mockClientDataResolver = _mocker.GetMock<IClientDataResolver>();
        mockClientDataResolver
            .Setup(m => m.IsHttps())
            .Returns(isHttps);
        mockClientDataResolver
            .Setup(m => m.GetHost())
            .Returns(host);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.RootUrl;

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }

    [TestMethod]
    public async Task GetBody_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        const string body = "THIS IS THE REQUEST BODY!";
        _mockHttpContext.SetBody(body);

        // Act
        var result = await service.GetBodyAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(body, result);
    }

    [TestMethod]
    public async Task GetBodyAsBytesAsync_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        var body = new byte[] {1, 2, 3};
        _mockHttpContext.SetBody(body);

        // Act
        var result = await service.GetBodyAsBytesAsync(CancellationToken.None);

        // Assert
        CollectionAssert.AreEqual(body, result);
    }

    [TestMethod]
    public void GetQueryStringDictionary_HappyFlow()
    {
        // Arrange
        var queryDict = new Dictionary<string, StringValues> {{"key1", "val1"}, {"key2", "val2"}};
        _mockHttpContext.SetQuery(queryDict);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.GetQueryStringDictionary();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("val1", result["key1"]);
        Assert.AreEqual("val2", result["key2"]);
    }

    [TestMethod]
    public void GetQueryString_HappyFlow()
    {
        // Arrange
        _mockHttpContext.SetQueryString("?key1=val1");
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.GetQueryString();

        // Assert
        Assert.AreEqual("?key1=val1", result);
    }

    [TestMethod]
    public void GetHeaders_HappyFlow()
    {
        // Arrange
        var headerDict = new Dictionary<string, StringValues> {{"key1", "val1"}, {"key2", "val2"}};
        _mockHttpContext.SetRequestHeaders(headerDict);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.GetHeaders();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("val1", result["key1"]);
        Assert.AreEqual("val2", result["key2"]);
    }

    [TestMethod]
    public void SetItem_GetItem_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.SetItem("key", 42);
        var result = service.GetItem<int>("key");

        // Assert
        Assert.AreEqual(1, _mockHttpContext.Items.Count);
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void GetFormValues_NoContentTypeSet_ShouldReturnEmptyArray()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.GetFormValues();

        // Assert
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void GetFormValues_ErrorWhileReadingForm_ShouldReturnEmptyArray()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        _mockHttpContext.SetRequestHeader(HeaderKeys.ContentType, MimeTypes.MultipartFormDataMime);
        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.Form)
            .Throws<InvalidOperationException>();

        // Act
        var result = service.GetFormValues();

        // Assert
        Assert.AreEqual(0, result.Length);
    }

    [DataTestMethod]
    [DataRow("application/x-www-form-urlencoded")]
    [DataRow("multipart/form-data")]
    [DataRow("APPLICATION/X-WWW-FORM-URLENCODED")]
    [DataRow("MULTIPART/FORM-DATA")]
    [DataRow("application/x-www-form-urlencoded; encoding=utf-8")]
    [DataRow("multipart/form-data; encoding=utf-8")]
    public void GetFormValues_HappyFlow(string contentType)
    {
        // Arrange
        var formDict = new Dictionary<string, StringValues> {{"key1", "val1"}, {"key2", "val2"}};
        _mockHttpContext.SetForm(formDict);
        _mockHttpContext.SetRequestHeader(HeaderKeys.ContentType, contentType);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = service.GetFormValues();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual("key1", result[0].Item1);
        Assert.AreEqual("val1", result[0].Item2.Single());

        Assert.AreEqual("key2", result[1].Item1);
        Assert.AreEqual("val2", result[1].Item2.Single());
    }

    [TestMethod]
    public void SetStatusCodeAsInt_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.SetStatusCode(204);

        // Assert
        Assert.AreEqual(204, _mockHttpContext.GetStatusCode());
    }

    [TestMethod]
    public void SetStatusCode_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.SetStatusCode(HttpStatusCode.NoContent);

        // Assert
        Assert.AreEqual(204, _mockHttpContext.GetStatusCode());
    }

    [TestMethod]
    public void AddHeader_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.AddHeader("key1", "val1");

        // Assert
        Assert.AreEqual("val1", _mockHttpContext.Response.Headers["key1"].Single());
    }

    [TestMethod]
    public void TryAddHeader_HeaderAlreadyAdded_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        _mockHttpContext.Response.Headers.Add("key1", "val1");

        // Act
        var result = service.TryAddHeader("key1", "val1");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryAddHeader_HeaderNotAddedYet_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        _mockHttpContext.Response.Headers.Add("key1", "val1");

        // Act
        var result = service.TryAddHeader("key2", "val2");

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("val2", _mockHttpContext.Response.Headers["key2"].Single());
    }

    [TestMethod]
    public void SetUser_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        var principal = new ClaimsPrincipal();

        // Act
        service.SetUser(principal);

        // Assert
        Assert.AreEqual(_mockHttpContext.User, principal);
    }

    [TestMethod]
    public void AbortConnection_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.AbortConnection();

        // Assert
        Assert.IsTrue(_mockHttpContext.AbortCalled);
    }
}
