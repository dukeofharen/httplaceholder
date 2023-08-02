using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Web.Shared.Tests.Authorization;

[TestClass]
public class LoginServiceFacts
{
    private readonly IOptionsMonitor<SettingsModel> _options = MockSettingsFactory.GetOptionsMonitor();
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize() => _mocker.Use(_options);

    [TestMethod]
    public void LoginService_CheckLoginCookie_NoUsernameAndPasswordSet_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginService>();

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_NoCookieSet_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginService>();
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashIncorrect_ShouldReturnFalse()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginService>();
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
    public void LoginService_CheckLoginCookie_UsernameAndPasswordSet_HashCorrect_ShouldReturnTrue()
    {
        // Arrange
        var service = _mocker.CreateInstance<LoginService>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        httpContextServiceMock
            .Setup(m => m.GetRequestCookie("HttPlaceholderLoggedin"))
            .Returns(new KeyValuePair<string, string>("HttPlaceholderLoggedin", "qkUYd4wTaLeznD/nN1v9ei9/5XUekWt1hyOctq3bQZ9DMhSk7FJz+l1ILk++kyYlu+VguxVcuEC9R4Ryk763GA=="));
        _options.CurrentValue.Authentication.ApiUsername = "user";
        _options.CurrentValue.Authentication.ApiPassword = "pass";

        // Act
        var result = service.CheckLoginCookie();

        // Assert
        Assert.IsTrue(result);
    }
}
