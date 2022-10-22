using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Tests.Authorization;

[TestClass]
public class ApiAuthorizationAttributeFacts
{
    private readonly ApiAuthorizationAttribute _attribute = new();
    private readonly Mock<IHttpContextService> _mockHttpContextService = new();
    private readonly MockLogger<ApiAuthorizationAttribute> _mockLogger = new();
    private readonly Mock<ILoginService> _mockLoginService = new();
    private readonly SettingsModel _settings = new() {Authentication = new AuthenticationSettingsModel()};

    [TestMethod]
    public void OnActionExecuting_LoginCookieSet_ShouldAddUserContext()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(true);
        const string username = "user";
        _settings.Authentication.ApiUsername = username;
        _settings.Authentication.ApiPassword = "password";

        ClaimsPrincipal capturedClaimsPrincipal = null;
        _mockHttpContextService
            .Setup(m => m.SetUser(It.IsAny<ClaimsPrincipal>()))
            .Callback<ClaimsPrincipal>(p => capturedClaimsPrincipal = p);

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUser(username, capturedClaimsPrincipal);
        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_LoginCookieNotSet_NoAuthorizationHeader()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string>());

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUserNotSet();
        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void OnActionExecuting_LoginCookieNotSet_BasicAuthIncorrectAmountOfParts()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjpwYXNzOm9uemlu"}});

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUserNotSet();
        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void OnActionExecuting_LoginCookieNotSet_UsernameAndPasswordIncorrect()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        _settings.Authentication.ApiUsername = "user";
        _settings.Authentication.ApiPassword = "pass";
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjE6cGFzczE="}});

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUserNotSet();
        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void OnActionExecuting_LoginCookieNotSet_ExceptionWhileParsingBasicAuth()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic ()*&"}});

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUserNotSet();
        Assert.IsInstanceOfType(context.Result, typeof(UnauthorizedResult));
        _mockLogger.ContainsWithExactText(LogLevel.Warning, "Error while parsing basic authentication.");
    }

    [TestMethod]
    public void OnActionExecuting_LoginCookieNotSet_UsernameAndPasswordCorrect()
    {
        // Arrange
        var context = CreateContext();
        _mockLoginService
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);
        _settings.Authentication.ApiUsername = "user";
        _settings.Authentication.ApiPassword = "pass";
        SetHeaders(new Dictionary<string, string> {{"Authorization", "Basic dXNlcjpwYXNz"}});

        ClaimsPrincipal capturedClaimsPrincipal = null;
        _mockHttpContextService
            .Setup(m => m.SetUser(It.IsAny<ClaimsPrincipal>()))
            .Callback<ClaimsPrincipal>(p => capturedClaimsPrincipal = p);

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        AssertUser("user", capturedClaimsPrincipal);
        Assert.IsNull(context.Result);
    }

    private void SetHeaders(IDictionary<string, string> headers) =>
        _mockHttpContextService
            .Setup(m => m.GetHeaders())
            .Returns(headers);

    private ActionExecutingContext CreateContext()
    {
        var httpContext = new MockHttpContext();
        httpContext.ServiceProviderMock
            .Setup(m => m.GetService(typeof(ILoginService)))
            .Returns(_mockLoginService.Object);
        httpContext.ServiceProviderMock
            .Setup(m => m.GetService(typeof(IOptions<SettingsModel>)))
            .Returns(Options.Create(_settings));
        httpContext.ServiceProviderMock
            .Setup(m => m.GetService(typeof(ILogger<ApiAuthorizationAttribute>)))
            .Returns(_mockLogger);
        httpContext.ServiceProviderMock
            .Setup(m => m.GetService(typeof(IHttpContextService)))
            .Returns(_mockHttpContextService.Object);
        return new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), new ModelStateDictionary()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            new object()) {HttpContext = httpContext};
    }

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
        _mockHttpContextService.Verify(m => m.SetUser(It.IsAny<ClaimsPrincipal>()), Times.Never);
}
