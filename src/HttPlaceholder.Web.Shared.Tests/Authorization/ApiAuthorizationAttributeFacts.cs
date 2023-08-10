using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Web.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace HttPlaceholder.Web.Shared.Tests.Authorization;

[TestClass]
public class ApiAuthorizationAttributeFacts
{
    private readonly ApiAuthorizationAttribute _attribute = new();
    private readonly Mock<IApiAuthorizationService> _mockApiAuthorizationService = new();

    [TestMethod]
    public void OnActionExecuting_AuthIsCorrect_ShouldNotSetResult()
    {
        // Arrange
        _mockApiAuthorizationService
            .Setup(m => m.CheckAuthorization())
            .Returns(true);
        var context = CreateContext();

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void OnActionExecuting_AuthIsInorrect_ShouldNotSetResultToUnauthorizedResult()
    {
        // Arrange
        _mockApiAuthorizationService
            .Setup(m => m.CheckAuthorization())
            .Returns(false);
        var context = CreateContext();

        // Act
        _attribute.OnActionExecuting(context);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(context.Result);
    }

    private ActionExecutingContext CreateContext()
    {
        var httpContext = new MockHttpContext();
        httpContext.ServiceProviderMock
            .Setup(m => m.GetService(typeof(IApiAuthorizationService)))
            .Returns(_mockApiAuthorizationService.Object);
        return new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), new ModelStateDictionary()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            new object()) {HttpContext = httpContext};
    }
}
