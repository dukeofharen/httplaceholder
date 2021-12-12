using HttPlaceholder.Infrastructure.Web;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Tests.Infrastructure.Web;

[TestClass]
public class ClientIpResolverFacts
{
    private readonly MockHttpContext _mockContext = new MockHttpContext();
    private ClientDataResolver _resolver;

    [TestInitialize]
    public void Setup()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockContext);
        _resolver = new ClientDataResolver(accessorMock.Object);
    }

    [TestMethod]
    public void ClientDataResolver_GetClientIp_IpIsLoopback_ForwardedHeaderSet_ShouldReturnForwardedIp()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        const string forwardedIp = "123.123.123.123";
        _mockContext.SetIp(loopbackIp);
        _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

        // Act
        var ip = _resolver.GetClientIp();

        // Assert
        Assert.AreEqual(forwardedIp, ip);
    }

    [TestMethod]
    public void ClientDataResolver_GetClientIp_IpIsLoopback_ForwardedHeaderNotSet_ShouldReturnLoopbackIp()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        // Act
        var ip = _resolver.GetClientIp();

        // Assert
        Assert.AreEqual(loopbackIp, ip);
    }

    [TestMethod]
    public void ClientDataResolver_GetClientIp_IpIsNotLoopback_ForwardedHeaderSet_ShouldReturnLoopbackIp()
    {
        // Arrange
        const string externalIp = "222.222.222.222";
        const string forwardedIp = "123.123.123.123";
        _mockContext.SetIp(externalIp);
        _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

        // Act
        var ip = _resolver.GetClientIp();

        // Assert
        Assert.AreEqual(externalIp, ip);
    }

    [DataTestMethod]
    [DataRow("127.0.0.1", true)]
    [DataRow("::1", true)]
    [DataRow("0000:0000:0000:0000:0000:0000:0000:0001", true)]
    [DataRow("::ffff:127.0.0.1", true)]
    [DataRow("128.0.0.1", false)]
    [DataRow("112.112.112.112", false)]
    public void ClientDataResolver_GetClientIp_LoopbackIps(string ip, bool isLoopback)
    {
        // Arrange
        const string forwardedIp = "123.123.123.123";
        _mockContext.SetIp(ip);
        _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

        // Act
        var result = _resolver.GetClientIp();

        // Assert
        Assert.AreEqual(isLoopback ? forwardedIp : ip, result);
    }

    [TestMethod]
    public void ClientDataResolver_GetHost_IpIsLoopback_ForwardedHeaderSet_ShouldReturnForwardedHost()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        var forwardedHost = "httplaceholder.com";
        _mockContext.Request.Headers.Add("X-Forwarded-Host", forwardedHost);

        // Act
        var host = _resolver.GetHost();

        // Assert
        Assert.AreEqual(forwardedHost, host);
    }

    [TestMethod]
    public void ClientDataResolver_GetHost_IpIsNotLoopback_ForwardedHeaderSet_ShouldReturnActualHost()
    {
        // Arrange
        const string loopbackIp = "111.111.111.111";
        _mockContext.SetIp(loopbackIp);

        const string forwardedHost = "httplaceholder.com";
        _mockContext.Request.Headers.Add("X-Forwarded-Host", forwardedHost);

        var actualHost = "localhost";
        _mockContext.SetHost(actualHost);

        // Act
        var host = _resolver.GetHost();

        // Assert
        Assert.AreEqual(actualHost, host);
    }

    [TestMethod]
    public void ClientDataResolver_GetHost_IpIsLoopback_ForwardedHeaderNotSet_ShouldReturnActualHost()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        const string actualHost = "localhost";
        _mockContext.SetHost(actualHost);

        // Act
        var host = _resolver.GetHost();

        // Assert
        Assert.AreEqual(actualHost, host);
    }

    [TestMethod]
    public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderSet_Https_ShouldReturnTrue()
    {
        // Arrange
        var loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        _mockContext.Request.Headers.Add("X-Forwarded-Proto", "https");

        // Act
        var isHttps = _resolver.IsHttps();

        // Assert
        Assert.IsTrue(isHttps);
    }

    [TestMethod]
    public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderSet_Http_ShouldReturnFalse()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        _mockContext.Request.Headers.Add("X-Forwarded-Proto", "http");

        // Act
        var isHttps = _resolver.IsHttps();

        // Assert
        Assert.IsFalse(isHttps);
    }

    [TestMethod]
    public void ClientDataResolver_IsHttps_IpIsNotLoopback_ForwardedHeaderNotSet_Https_ShouldReturnTrue()
    {
        // Arrange
        const string loopbackIp = "123.123.123.123";
        _mockContext.SetIp(loopbackIp);

        _mockContext.SetHttps(true);

        // Act
        var isHttps = _resolver.IsHttps();

        // Assert
        Assert.IsTrue(isHttps);
    }

    [TestMethod]
    public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderNotSet_Http_ShouldReturnTrue()
    {
        // Arrange
        const string loopbackIp = "123.123.123.123";
        _mockContext.SetIp(loopbackIp);

        _mockContext.SetHttps(false);

        // Act
        var isHttps = _resolver.IsHttps();

        // Assert
        Assert.IsFalse(isHttps);
    }

    [TestMethod]
    public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderNotSet_Https_ShouldReturnTrue()
    {
        // Arrange
        const string loopbackIp = "127.0.0.1";
        _mockContext.SetIp(loopbackIp);

        _mockContext.SetHttps(true);

        // Act
        var isHttps = _resolver.IsHttps();

        // Assert
        Assert.IsTrue(isHttps);
    }
}