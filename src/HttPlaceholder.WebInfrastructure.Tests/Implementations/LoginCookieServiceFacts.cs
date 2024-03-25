using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.WebInfrastructure.Implementations;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.WebInfrastructure.Tests.Implementations;

[TestClass]
public class LoginCookieServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();

    [TestInitialize]
    public void Initialize() => _mocker.Use(_options);

    [TestMethod]
    public void CheckLoginCookie_NoUsernameAndPasswordSet_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginCookieService>();

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckLoginCookie_UsernameAndPasswordSet_NoCookieSet_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginCookieService>();
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CheckLoginCookie_UsernameAndPasswordSet_HashIncorrect_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginCookieService>();
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        httpContextServiceMock
            .Setup(m => m.GetRequestCookie("HttPlaceholderLoggedin"))
            .Returns(new KeyValuePair<string, string>("HttPlaceholderLoggedin", "INCORRECT"));

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CheckLoginCookie_UsernameAndPasswordSet_HashCorrect_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginCookieService>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        httpContextServiceMock
            .Setup(m => m.GetRequestCookie("HttPlaceholderLoggedin"))
            .Returns(new KeyValuePair<string, string>("HttPlaceholderLoggedin",
                "qkUYd4wTaLeznD/nN1v9ei9/5XUekWt1hyOctq3bQZ9DMhSk7FJz+l1ILk++kyYlu+VguxVcuEC9R4Ryk763GA=="));
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }
}
