using HttPlaceholder.Application.Configuration;
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
    [DataRow(false, "httplaceholder.com", "/path", "?key1=val1", null, "http://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", "?key1=val1", null, "https://httplaceholder.com/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", null, null, "https://httplaceholder.com/path")]
    [DataRow(true, "httplaceholder.com", "/path", "?key1=val1", "https://example.com/stubs/", "https://example.com/stubs/path?key1=val1")]
    [DataRow(true, "httplaceholder.com", "/path", "?key1=val1", "https://example.com/stubs", "https://example.com/stubs/path?key1=val1")]
    public void GetDisplayUrl_HappyFlow(
        bool isHttps,
        string host,
        string path,
        string queryString,
        string configuredPublicUrl,
        string expectedUrl)
    {
        // Arrange
        _mockHttpContext.SetRequestPath(path);
        _mockHttpContext.SetQueryString(queryString);

        if (!string.IsNullOrWhiteSpace(configuredPublicUrl))
        {
            var options = MockSettingsFactory.GetOptionsMonitor(new SettingsModel
            {
                Web = new WebSettingsModel() {PublicUrl = configuredPublicUrl}
            });
            _mocker.Use(options);
        }
        else
        {
            var mockClientDataResolver = _mocker.GetMock<IClientDataResolver>();
            mockClientDataResolver
                .Setup(m => m.IsHttps())
                .Returns(isHttps);
            mockClientDataResolver
                .Setup(m => m.GetHost())
                .Returns(host);
        }

        var resolver = _mocker.CreateInstance<UrlResolver>();

        // Act
        var result = resolver.GetDisplayUrl();

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }

    [DataTestMethod]
    [DataRow(false, "httplaceholder.com", null, "http://httplaceholder.com")]
    [DataRow(true, "httplaceholder.com", null, "https://httplaceholder.com")]
    [DataRow(true, "httplaceholder.com", "https://example.com/stubs", "https://example.com/stubs")]
    [DataRow(true, "httplaceholder.com", "https://example.com/stubs/", "https://example.com/stubs")]
    public void GetRootUrl_HappyFlow(
        bool isHttps,
        string host,
        string configuredPublicUrl,
        string expectedUrl)
    {
        // Arrange
        if (!string.IsNullOrWhiteSpace(configuredPublicUrl))
        {
            var options = MockSettingsFactory.GetOptionsMonitor(new SettingsModel
            {
                Web = new WebSettingsModel() {PublicUrl = configuredPublicUrl}
            });
            _mocker.Use(options);
        }
        else
        {
            var mockClientDataResolver = _mocker.GetMock<IClientDataResolver>();
            mockClientDataResolver
                .Setup(m => m.IsHttps())
                .Returns(isHttps);
            mockClientDataResolver
                .Setup(m => m.GetHost())
                .Returns(host);
        }

        var resolver = _mocker.CreateInstance<UrlResolver>();

        // Act
        var result = resolver.GetRootUrl();

        // Assert
        Assert.AreEqual(expectedUrl, result);
    }
}
