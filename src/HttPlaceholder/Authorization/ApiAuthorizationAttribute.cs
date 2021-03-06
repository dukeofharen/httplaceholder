﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Authorization
{
    public class ApiAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var loginService = context.HttpContext.RequestServices.GetService<ILoginService>();
            var settings = context.HttpContext.RequestServices.GetService<IOptions<SettingsModel>>().Value;

            bool result;
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ApiAuthorizationAttribute>>();
            var username = settings.Authentication?.ApiUsername ?? string.Empty;
            var password = settings.Authentication?.ApiPassword ?? string.Empty;
            if (loginService.CheckLoginCookie())
            {
                AddUserContext(context, username);
                return;
            }

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                // Try to retrieve basic auth header here.
                context.HttpContext.Request.Headers.TryGetValue("Authorization", out var values);
                var value = values.FirstOrDefault();
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
            }
            else
            {
                result = true;
            }

            if (!result)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                // Everything went OK, so let's add the user to the claims.
                AddUserContext(context, username);
                loginService.SetLoginCookie(username, password);
            }
        }

        [SuppressMessage(
            "SonarQube",
            "S4834",
            Justification = "There are only 2 permissions, API access or no API access")]
        private static void AddUserContext(ActionContext context, string username) =>
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }));
    }
}
