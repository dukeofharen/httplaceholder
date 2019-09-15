using HttPlaceholder.Infrastructure.Implementations;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Infrastructure.Tests.Implementations
{
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
            string loopbackIp = "127.0.0.1";
            string forwardedIp = "123.123.123.123";
            _mockContext.SetIp(loopbackIp);
            _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

            // Act
            string ip = _resolver.GetClientIp();

            // Assert
            Assert.AreEqual(forwardedIp, ip);
        }

        [TestMethod]
        public void ClientDataResolver_GetClientIp_IpIsLoopback_ForwardedHeaderNotSet_ShouldReturnLoopbackIp()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            // Act
            string ip = _resolver.GetClientIp();

            // Assert
            Assert.AreEqual(loopbackIp, ip);
        }

        [TestMethod]
        public void ClientDataResolver_GetClientIp_IpIsNotLoopback_ForwardedHeaderSet_ShouldReturnLoopbackIp()
        {
            // Arrange
            string externalIp = "222.222.222.222";
            string forwardedIp = "123.123.123.123";
            _mockContext.SetIp(externalIp);
            _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

            // Act
            string ip = _resolver.GetClientIp();

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
            string forwardedIp = "123.123.123.123";
            _mockContext.SetIp(ip);
            _mockContext.Request.Headers.Add("X-Forwarded-For", $"{forwardedIp}, 127.0.0.1");

            // Act
            string result = _resolver.GetClientIp();

            // Assert
            Assert.AreEqual(isLoopback ? forwardedIp : ip, result);
        }

        [TestMethod]
        public void ClientDataResolver_GetHost_IpIsLoopback_ForwardedHeaderSet_ShouldReturnForwardedHost()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            string forwardedHost = "httplaceholder.com";
            _mockContext.Request.Headers.Add("X-Forwarded-Host", forwardedHost);

            // Act
            string host = _resolver.GetHost();

            // Assert
            Assert.AreEqual(forwardedHost, host);
        }

        [TestMethod]
        public void ClientDataResolver_GetHost_IpIsNotLoopback_ForwardedHeaderSet_ShouldReturnActualHost()
        {
            // Arrange
            string loopbackIp = "111.111.111.111";
            _mockContext.SetIp(loopbackIp);

            string forwardedHost = "httplaceholder.com";
            _mockContext.Request.Headers.Add("X-Forwarded-Host", forwardedHost);

            string actualHost = "localhost";
            _mockContext.SetHost(actualHost);

            // Act
            string host = _resolver.GetHost();

            // Assert
            Assert.AreEqual(actualHost, host);
        }

        [TestMethod]
        public void ClientDataResolver_GetHost_IpIsLoopback_ForwardedHeaderNotSet_ShouldReturnActualHost()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            string actualHost = "localhost";
            _mockContext.SetHost(actualHost);

            // Act
            string host = _resolver.GetHost();

            // Assert
            Assert.AreEqual(actualHost, host);
        }

        [TestMethod]
        public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderSet_Https_ShouldReturnTrue()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            _mockContext.Request.Headers.Add("X-Forwarded-Proto", "https");

            // Act
            bool isHttps = _resolver.IsHttps();

            // Assert
            Assert.IsTrue(isHttps);
        }

        [TestMethod]
        public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderSet_Http_ShouldReturnFalse()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            _mockContext.Request.Headers.Add("X-Forwarded-Proto", "http");

            // Act
            bool isHttps = _resolver.IsHttps();

            // Assert
            Assert.IsFalse(isHttps);
        }

        [TestMethod]
        public void ClientDataResolver_IsHttps_IpIsNotLoopback_ForwardedHeaderNotSet_Https_ShouldReturnTrue()
        {
            // Arrange
            string loopbackIp = "123.123.123.123";
            _mockContext.SetIp(loopbackIp);

            _mockContext.SetHttps(true);

            // Act
            bool isHttps = _resolver.IsHttps();

            // Assert
            Assert.IsTrue(isHttps);
        }

        [TestMethod]
        public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderNotSet_Http_ShouldReturnTrue()
        {
            // Arrange
            string loopbackIp = "123.123.123.123";
            _mockContext.SetIp(loopbackIp);

            _mockContext.SetHttps(false);

            // Act
            bool isHttps = _resolver.IsHttps();

            // Assert
            Assert.IsFalse(isHttps);
        }

        [TestMethod]
        public void ClientDataResolver_IsHttps_IpIsLoopback_ForwardedHeaderNotSet_Https_ShouldReturnTrue()
        {
            // Arrange
            string loopbackIp = "127.0.0.1";
            _mockContext.SetIp(loopbackIp);

            _mockContext.SetHttps(true);

            // Act
            bool isHttps = _resolver.IsHttps();

            // Assert
            Assert.IsTrue(isHttps);
        }
    }
}
