using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HttPlaceholder.Web.Shared.Authorization;

/// <summary>
///     An attribute that is used to check the authentication and authorization for all API requests.
/// </summary>
public class ApiAuthorizationAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var apiAuthService = httpContext.RequestServices.GetRequiredService<IApiAuthorizationService>();
        var result = apiAuthService.CheckAuthorization();
        if (!result)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
