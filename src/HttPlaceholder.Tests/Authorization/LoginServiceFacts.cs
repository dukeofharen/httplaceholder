using System.Collections.Generic;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Authorization.Implementations;
using HttPlaceholder.TestUtilities.Http;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Tests.Authorization;

[TestClass]
public class LoginServiceFacts
{
    private readonly Dictionary<string, string> _cookies = new();
    private readonly MockHttpContext _mockHttpContext = new();
    private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
    private LoginService _service;

    [TestInitialize]
    public void Initialize()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock
            .Setup(m => m.HttpContext)
            .Returns(_mockHttpContext);

        _mockHttpContext
            .HttpRequestMock
            .Setup(m => m.Cookies)
            .Returns(() => new RequestCookieCollection(_cookies));

        _service = new LoginService(accessorMock.Object, _options);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_NoUsernameAndPasswordSet_ShouldReturnTrue()
    {
        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_NoCookieSet_ShouldReturnFalse()
    {
        // Arrange
        _options.Value.Authentication.ApiUsername = "user";
        _options.Value.Authentication.ApiPassword = "pass";

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashIncorrect_ShouldReturnFalse()
    {
        // Arrange
        _options.Value.Authentication.ApiUsername = "user";
        _options.Value.Authentication.ApiPassword = "pass";
        _cookies.Add(CookieKeys.LoginCookieKey, "INCORRECT");

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashCorrect_ShouldReturnTrue()
    {
        // Arrange
        _options.Value.Authentication.ApiUsername = "user";
        _options.Value.Authentication.ApiPassword = "pass";
        _cookies.Add(CookieKeys.LoginCookieKey, "qkUYd4wTaLeznD/nN1v9ei9/5XUekWt1hyOctq3bQZ9DMhSk7FJz+l1ILk++kyYlu+VguxVcuEC9R4Ryk763GA==");

        // Act
        var result = _service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }
}