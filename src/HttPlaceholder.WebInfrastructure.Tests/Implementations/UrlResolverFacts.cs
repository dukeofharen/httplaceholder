using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.WebInfrastructure.Implementations;

namespace HttPlaceholder.WebInfrastructure.Tests.Implementations;

[TestClass]
public class UrlResolverFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockHttpContext _mockHttpContext = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use(TestObjectFactory.CreateHttpContextAccessor(_mockHttpContext));

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow(false, "httplaceholder.com", "/path", "?key1=val1", "http://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", "?key1=val1", "https://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", null, "https://httplaceholder.com/path")]
    public void GetDisplayUrl_HappyFlow(bool isHttps, string host, string path, string queryString, string expectedUrl)
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

        var resolver = _mocker.CreateInstance<UrlResolver>();

        // Act
        var result = resolver.GetDisplayUrl();

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }

    [DataTestMethod]
    [DataRow(false, "httplaceholder.com", "http://httplaceholder.com")]
    [DataRow(true, "httplaceholder.com", "https://httplaceholder.com")]
    public void GetRootUrl_HappyFlow(bool isHttps, string host, string expectedUrl)
    {
        // Arrange
        var mockClientDataResolver = _mocker.GetMock<IClientDataResolver>();
        mockClientDataResolver
            .Setup(m => m.IsHttps())
            .Returns(isHttps);
        mockClientDataResolver
            .Setup(m => m.GetHost())
            .Returns(host);

        var resolver = _mocker.CreateInstance<UrlResolver>();

        // Act
        var result = resolver.GetRootUrl();

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }
}
