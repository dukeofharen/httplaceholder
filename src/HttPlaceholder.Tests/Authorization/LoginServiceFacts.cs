using Ducode.Essentials.Mvc.TestUtilities;
using HttPlaceholder.Authorization.Implementations;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Text;

namespace HttPlaceholder.Tests.Authorization
{
    [TestClass]
    public class LoginServiceFacts
    {
        private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _config = new Dictionary<string, string>();
        private readonly Mock<IConfigurationService> _configurationServiceMock = new Mock<IConfigurationService>();
        private readonly MockHttpContext _mockHttpContext = new MockHttpContext();
        private LoginService _service;

        [TestInitialize]
        public void Initialize()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock
                .Setup(m => m.HttpContext)
                .Returns(_mockHttpContext);

            _configurationServiceMock
                .Setup(m => m.GetConfiguration())
                .Returns(_config);

            _mockHttpContext
                .HttpRequestMock
                .Setup(m => m.Cookies)
                .Returns(() => new RequestCookieCollection(_cookies));

            _service = new LoginService(_configurationServiceMock.Object, accessorMock.Object);
        }

        [TestMethod]
        public void LoginService_CheckLoginCookie_NoUsernameAndPasswordSet_ShouldReturnTrue()
        {
            // Act
            bool result = _service.CheckLoginCookie();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_NoCookieSet_ShouldReturnFalse()
        {
            // Arrange
            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "user");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "pass");

            // Act
            bool result = _service.CheckLoginCookie();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashIncorrect_ShouldReturnFalse()
        {
            // Arrange
            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "user");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "pass");
            _cookies.Add(Constants.CookieKeys.LoginCookieKey, "INCORRECT");

            // Act
            bool result = _service.CheckLoginCookie();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashCorrect_ShouldReturnTrue()
        {
            // Arrange
            _config.Add(Constants.ConfigKeys.ApiUsernameKey, "user");
            _config.Add(Constants.ConfigKeys.ApiPasswordKey, "pass");
            _cookies.Add(Constants.CookieKeys.LoginCookieKey, "qkUYd4wTaLeznD/nN1v9ei9/5XUekWt1hyOctq3bQZ9DMhSk7FJz+l1ILk++kyYlu+VguxVcuEC9R4Ryk763GA==");

            // Act
            bool result = _service.CheckLoginCookie();

            // Assert
            Assert.IsTrue(result);
        }
    }
}
