using System.Net;
using System.Security.Claims;
using HttPlaceholder.WebInfrastructure.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.WebInfrastructure.Tests.Implementations;

[TestClass]
public class HttpContextServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockHttpContext _mockHttpContext = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use(TestObjectFactory.CreateHttpContextAccessor(_mockHttpContext));

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

        var body = new byte[] { 1, 2, 3 };
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
        var queryDict = new Dictionary<string, StringValues> { { "key1", "val1" }, { "key2", "val2" } };
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
        var headerDict = new Dictionary<string, StringValues> { { "key1", "val1" }, { "key2", "val2" } };
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
    public void SetItemTwice_GetItem_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.SetItem("key", 42);
        service.SetItem("key", 43);
        var result = service.GetItem<int>("key");

        // Assert
        Assert.AreEqual(1, _mockHttpContext.Items.Count);
        Assert.AreEqual(43, result);
    }

    [TestMethod]
    public void DeleteItem_ItemDoesntExist_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        service.SetItem("key1", 42);

        // Act
        var result = service.DeleteItem("key2");

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(1, _mockHttpContext.Items.Count);
    }

    [TestMethod]
    public void DeleteItem_ItemExists_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        service.SetItem("key1", 42);

        // Act
        var result = service.DeleteItem("key1");

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_mockHttpContext.Items.Any());
    }

    [TestMethod]
    public async Task GetFormValues_NoContentTypeSet_ShouldReturnEmptyArray()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.HasFormContentType)
            .Returns(false);

        // Act
        var result = await service.GetFormValuesAsync();

        // Assert
        Assert.AreEqual(0, result.Length);
        _mockHttpContext.HttpRequestMock.Verify(m => m.ReadFormAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task GetFormValues_ErrorWhileReadingForm_ShouldReturnEmptyArray()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpContextService>();
        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.Form)
            .Throws<InvalidOperationException>();

        // Act
        var result = await service.GetFormValuesAsync();

        // Assert
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public async Task GetFormValues_HappyFlow()
    {
        // Arrange
        var formDict = new Dictionary<string, StringValues> { { "key1", "val1" }, { "key2", "val2" } };
        _mockHttpContext.SetForm(formDict);
        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.HasFormContentType)
            .Returns(true);

        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        var result = await service.GetFormValuesAsync();

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

        _mockHttpContext.Response.Headers.Append("key1", "val1");

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

        _mockHttpContext.Response.Headers.Append("key1", "val1");

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

    [TestMethod]
    public void AppendCookie_HappyFlow()
    {
        // Arrange
        const string key = "cookieKey";
        const string value = "cookieValue";
        var options = new CookieOptions();
        var service = _mocker.CreateInstance<HttpContextService>();

        // Act
        service.AppendCookie(key, value, options);

        // Assert
        _mockHttpContext
            .ResponseCookiesMock
            .Verify(m => m.Append(key, value, options));
    }

    [TestMethod]
    public void GetRequestCookie_HappyFlow()
    {
        // Arrange
        const string key = "cookieKey";
        const string expectedValue = "cookieValue";
        var service = _mocker.CreateInstance<HttpContextService>();

        _mockHttpContext.RequestCookieCollection.Add(key, expectedValue);

        // Act
        var result = service.GetRequestCookie(key);

        // Assert
        Assert.IsTrue(result.HasValue);
        Assert.AreEqual(key, result.Value.Key);
        Assert.AreEqual(expectedValue, result.Value.Value);
    }

    [TestMethod]
    public void GetRequestCookie_CookieNotFound()
    {
        // Arrange
        const string key = "cookieKey";
        const string expectedValue = "cookieValue";
        var service = _mocker.CreateInstance<HttpContextService>();

        _mockHttpContext.RequestCookieCollection.Add(key + "1", expectedValue);

        // Act
        var result = service.GetRequestCookie(key);

        // Assert
        Assert.IsNull(result);
    }
}
