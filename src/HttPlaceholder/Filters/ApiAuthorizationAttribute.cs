using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Ducode.Essentials.Console;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Filters
{
    public class ApiAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool result;
            var configurationService = context.HttpContext.RequestServices.GetService<IConfigurationService>();
            var config = configurationService.GetConfiguration();
            string username = config.GetValue(Constants.ConfigKeys.ApiUsernameKey, string.Empty);
            string password = config.GetValue(Constants.ConfigKeys.ApiPasswordKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                // Try to retrieve basic auth header here.
                context.HttpContext.Request.Headers.TryGetValue("Authorization", out var values);
                string value = values.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(value))
                {
                    result = false;
                }
                else
                {
                    value = value.Replace("Basic ", string.Empty);
                    string basicAuth = Encoding.UTF8.GetString(Convert.FromBase64String(value));
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
                // Everything went OK, so put let's add the username to the claims.
                context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, username)
                }));
            }
        }
    }
}