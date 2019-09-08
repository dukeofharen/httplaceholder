using Ducode.Essentials.Mvc.TestUtilities;
using HttPlaceholder.Infrastructure.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Infrastructure.Tests.Implementations
{
    [TestClass]
    public class ClientIpResolverFacts
    {
        private readonly MockHttpContext _mockContext = new MockHttpContext();
        private ClientIpResolver _resolver;

        [TestInitialize]
        public void Setup()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock
                .Setup(m => m.HttpContext)
                .Returns(_mockContext);
            _resolver = new ClientIpResolver(accessorMock.Object);
        }

        [TestMethod]
        public void ClientIpResolver_GetClientIp_IpIsLoopback_ForwardedHeaderSet_ShouldReturnForwardedIp()
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
        public void ClientIpResolver_GetClientIp_IpIsLoopback_ForwardedHeaderNotSet_ShouldReturnLoopbackIp()
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
        public void ClientIpResolver_GetClientIp_IpIsNotLoopback_ForwardedHeaderSet_ShouldReturnLoopbackIp()
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
        public void ClientIpResolver_GetClientIp_LoopbackIps(string ip, bool isLoopback)
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
    }
}
