using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Infrastructure.Web;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Tests.Infrastructure.Web;

[TestClass]
public class ClientIpResolverFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockHttpContext _mockHttpContext = new();
    private readonly SettingsModel _settings = new() {Web = new WebSettingsModel()};

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
        var httpContextAccessorMock = _mocker.GetMock<IHttpContextAccessor>();
        httpContextAccessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockHttpContext);
    }

    [DataTestMethod]
    [DataRow(false, "1.2.3.4", "8.8.8.8", "", "1.2.3.4")]
    [DataRow(true, "1.2.3.4", "8.8.8.8", "", "1.2.3.4")]
    [DataRow(true, "1.2.3.4", "8.8.8.8", "2.2.2.2", "1.2.3.4")]
    [DataRow(true, "1.2.3.4", "8.8.8.8", "1.2.3.4", "8.8.8.8")]
    [DataRow(true, "1.2.3.4", "8.8.8.8", "1.2.3.4,5.6.7.8", "8.8.8.8")]
    [DataRow(true, "1.2.3.4", "7.7.7.7, 8.8.8.8", "1.2.3.4,5.6.7.8", "7.7.7.7")]
    [DataRow(true, "10.0.0.40", "8.8.8.8", "10.0.0.0/24", "8.8.8.8")]
    [DataRow(true, "127.0.0.1", "8.8.8.8", "", "8.8.8.8")]
    [DataRow(true, "::ffff:127.0.0.1", "8.8.8.8", "", "8.8.8.8")]
    public void GetClientIp_HappyFlow(
        bool shouldReadProxyHeaders,
        string actualIp,
        string xHeaderValue,
        string whitelistedIps,
        string expectedIp)
    {
        // Arrange
        var resolver = _mocker.CreateInstance<ClientDataResolver>();
        _settings.Web.ReadProxyHeaders = shouldReadProxyHeaders;
        _settings.Web.SafeProxyIps = whitelistedIps;
        _mockHttpContext.SetIp(actualIp);
        _mockHttpContext.Request.Headers.Add("x-forwarded-for", xHeaderValue);

        // Act
        var result = resolver.GetClientIp();

        // Assert
        Assert.AreEqual(expectedIp, result);
    }

    [DataTestMethod]
    [DataRow(false, "1.2.3.4", "localhost", "xhost", "", "localhost")]
    [DataRow(true, "1.2.3.4", "localhost", "xhost", "", "localhost")]
    [DataRow(true, "1.2.3.4", "localhost", "xhost", "2.2.2.2", "localhost")]
    [DataRow(true, "1.2.3.4", "localhost", "xhost", "1.2.3.4", "xhost")]
    [DataRow(true, "1.2.3.4", "localhost", "xhost", "1.2.3.4,5.6.7.8", "xhost")]
    [DataRow(true, "1.2.3.4", "localhost", "xhost1, xhost2", "1.2.3.4", "xhost1")]
    [DataRow(true, "10.0.0.40", "localhost", "xhost", "10.0.0.0/24", "xhost")]
    [DataRow(true, "127.0.0.1", "localhost", "xhost", "1.2.3.4", "xhost")]
    [DataRow(true, "::ffff:127.0.0.1", "localhost", "xhost", "1.2.3.4", "xhost")]
    public void GetHost_HappyFlow(
        bool shouldReadProxyHeaders,
        string actualIp,
        string actualHost,
        string xHeaderValue,
        string whitelistedIps,
        string expectedHost)
    {
        // Arrange
        var resolver = _mocker.CreateInstance<ClientDataResolver>();
        _settings.Web.ReadProxyHeaders = shouldReadProxyHeaders;
        _settings.Web.SafeProxyIps = whitelistedIps;
        _mockHttpContext.SetIp(actualIp);
        _mockHttpContext.SetHost(actualHost);
        _mockHttpContext.Request.Headers.Add("x-forwarded-host", xHeaderValue);

        // Act
        var result = resolver.GetHost();

        // Assert
        Assert.AreEqual(expectedHost, result);
    }

    [DataTestMethod]
    [DataRow(false, "1.2.3.4", true, "", "", true)]
    [DataRow(false, "1.2.3.4", false, "", "", false)]
    [DataRow(true, "1.2.3.4", false, "https", "2.2.2.2", false)]
    [DataRow(true, "1.2.3.4", false, "https", "1.2.3.4", true)]
    [DataRow(true, "1.2.3.4", false, "https", "1.2.3.4,5.6.7.8", true)]
    [DataRow(true, "1.2.3.4", false, "http, https", "1.2.3.4,5.6.7.8", false)]
    [DataRow(true, "10.0.0.40", false, "https", "10.0.0.0/24", true)]
    [DataRow(true, "127.0.0.1", false, "https", "1.2.3.4", true)]
    [DataRow(true, "::ffff:127.0.0.1", false, "https", "1.2.3.4", true)]
    public void IsHttps_HappyFlow(
        bool shouldReadProxyHeaders,
        string actualIp,
        bool actualHttps,
        string xHeaderValue,
        string whitelistedIps,
        bool expectedHttps)
    {
        // Arrange
        var resolver = _mocker.CreateInstance<ClientDataResolver>();
        _settings.Web.ReadProxyHeaders = shouldReadProxyHeaders;
        _settings.Web.SafeProxyIps = whitelistedIps;
        _mockHttpContext.SetIp(actualIp);
        _mockHttpContext.SetHttps(actualHttps);
        _mockHttpContext.Request.Headers.Add("x-forwarded-proto", xHeaderValue);

        // Act
        var result = resolver.IsHttps();

        // Assert
        Assert.AreEqual(expectedHttps, result);
    }
}
