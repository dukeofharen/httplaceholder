using System;
using System.Security.Claims;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Authorization;

/// <summary>
/// An attribute that is used to check the authentication and authorization for all API requests.
/// </summary>
public class ApiAuthorizationAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var requestServices = httpContext.RequestServices;

        var loginService = requestServices.GetRequiredService<ILoginService>();
        var settings = requestServices.GetRequiredService<IOptions<SettingsModel>>().Value;
        var logger = requestServices.GetRequiredService<ILogger<ApiAuthorizationAttribute>>();
        var httpContextService = requestServices.GetRequiredService<IHttpContextService>();

        bool result;
        var username = settings.Authentication?.ApiUsername ?? string.Empty;
        var password = settings.Authentication?.ApiPassword ?? string.Empty;
        if (loginService.CheckLoginCookie())
        {
            AddUserContext(httpContextService, username);
            return;
        }

        // Try to retrieve basic auth header here.
        httpContextService.GetHeaders().TryGetValue("Authorization", out var value);
        if (string.IsNullOrWhiteSpace(value))
        {
            result = false;
        }
        else
        {
            try
            {
                value = value.Replace("Basic ", string.Empty);
                var basicAuth = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                var parts = basicAuth.Split(':');
                if (parts.Length != 2)
                {
                    result = false;
                }
                else
                {
                    result = username == parts[0] && password == parts[1];
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.LogWarning(ex, "Error while parsing basic authentication.");
            }
        }

        if (!result)
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            // Everything went OK, so let's add the user to the claims.
            AddUserContext(httpContextService, username);
            loginService.SetLoginCookie(username, password);
        }
    }

    private static void AddUserContext(IHttpContextService httpContextService, string username) =>
        httpContextService.SetUser(
            new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, username)})));
}
