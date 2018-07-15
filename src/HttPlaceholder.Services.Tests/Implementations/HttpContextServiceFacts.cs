using HttPlaceholder.Services.Implementations;
using HttPlaceholder.TestUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Services.Tests.Implementations
{
   [TestClass]
   public class HttpContextServiceFacts
   {
      private Mock<IHttpContextAccessor> _httpContextAccessorMock;
      private HttpContextService _service;
      private MockHttpContext _mockHttpContext;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
         _service = new HttpContextService(_httpContextAccessorMock.Object);

         _mockHttpContext = new MockHttpContext();
         _httpContextAccessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockHttpContext);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextAccessorMock.VerifyAll();
      }

      [TestMethod]
      public void HttpContextService_GetClientIp_NoXForwardedHeaderSet_ShouldReturnRegularClientIp()
      {
         // arrange
         string ip = "1.2.3.4";

         _mockHttpContext.SetIp(ip);

         // act
         string result = _service.GetClientIp();

         // assert
         Assert.AreEqual(ip, result);
      }

      [TestMethod]
      public void HttpContextService_GetClientIp_XForwardedHeaderSet_ShouldReturnIpFromXForwardedHeader()
      {
         // arrange
         string ip = "1.2.3.4";
         string xForwardedIp = "11.22.33.44";

         _mockHttpContext.SetIp(ip);
         _mockHttpContext.Request.Headers.Add("X-Forwarded-For", $"{xForwardedIp}, 3.3.3.3, 4.4.4.4");

         // act
         string result = _service.GetClientIp();

         // assert
         Assert.AreEqual(xForwardedIp, result);
      }
   }
}
