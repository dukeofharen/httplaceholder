using System.Security.Claims;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.WebInfrastructure.Implementations;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.WebInfrastructure.Tests.Implementations;

[TestClass]
public class ApiAuthorizationServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockLogger<ApiAuthorizationService> _mockLogger = new();
    private readonly SettingsModel _settings = new() {Authentication = new AuthenticationSettingsModel()};

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<ILogger<ApiAuthorizationService>>(_mockLogger);
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
    }

    [TestMethod]
    public void LoginCookieSet_ShouldAddUserContext()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(true);
        const string username = "user";
        _settings.Authentication.ApiUsername = username;
        _settings.Authentication.ApiPassword = "password";

        ClaimsPrincipal capturedClaimsPrincipal = null;
        mockHttpContextService
            .Setup(m => m.SetUser(It.IsAny<ClaimsPrincipal>()))
            .Callback<ClaimsPrincipal>(p => capturedClaimsPrincipal = p);

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsTrue(result);
        AssertUser(username, capturedClaimsPrincipal);
    }

    [TestMethod]
    public void LoginCookieNotSet_NoAuthorizationHeader()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string>());

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsFalse(result);
        AssertUserNotSet();
    }

    [TestMethod]
    public void LoginCookieNotSet_BasicAuthIncorrectAmountOfParts()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjpwYXNzOm9uemlu"}});

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsFalse(result);
        AssertUserNotSet();
    }

    [TestMethod]
    public void LoginCookieNotSet_UsernameAndPasswordIncorrect()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        _settings.Authentication.ApiUsername = "user";
        _settings.Authentication.ApiPassword = "pass";
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjE6cGFzczE="}});

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsFalse(result);
        AssertUserNotSet();
    }

    [TestMethod]
    public void LoginCookieNotSet_ExceptionWhileParsingBasicAuth()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic ()*&"}});

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsFalse(result);
        AssertUserNotSet();
        _mockLogger.ContainsWithExactText(LogLevel.Warning, "Error while parsing basic authentication.");
    }

    [TestMethod]
    public void LoginCookieNotSet_UsernameAndPasswordCorrect()
    {
        // Arrange
        var service = _mocker.CreateInstance<ApiAuthorizationService>();
        var mockLoginService = _mocker.GetMock<ILoginCookieService>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        _settings.Authentication.ApiUsername = "user";
        _settings.Authentication.ApiPassword = "pass";
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjpwYXNz"}});

        ClaimsPrincipal capturedClaimsPrincipal = null;
        mockHttpContextService
            .Setup(m => m.SetUser(It.IsAny<ClaimsPrincipal>()))
            .Callback<ClaimsPrincipal>(p => capturedClaimsPrincipal = p);

        // Act
        var result = service.CheckAuthorization();

        // Assert
        Assert.IsTrue(result);
        AssertUser("user", capturedClaimsPrincipal);
    }

    private void SetHeaders(IDictionary<string, string> headers) =>
        _mocker.GetMock<IHttpContextService>()
            .Setup(m => m.GetHeaders())
            .Returns(headers);

    private static void AssertUser(string username, ClaimsPrincipal principal)
    {
        Assert.IsNotNull(principal);

        var identity = principal.Identity;
        Assert.IsNotNull(identity);
        Assert.IsInstanceOfType(identity, typeof(ClaimsIdentity));

        var claimsIdentity = (ClaimsIdentity)identity;
        var claims = claimsIdentity.Claims.ToArray();
        Assert.AreEqual(1, claims.Length);
        Assert.AreEqual(username, claims.Single(c => c.Type == ClaimTypes.Name).Value);
    }

    private void AssertUserNotSet() =>
        _mocker.GetMock<IHttpContextService>()
            .Verify(m => m.SetUser(It.IsAny<ClaimsPrincipal>()), Times.Never);
}
