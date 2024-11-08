using System;
using System.Security.Claims;
using System.Text;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Infrastructure.Web;

internal class ApiAuthorizationService(
    ILoginCookieService loginCookieService,
    ILogger<ApiAuthorizationService> logger,
    IHttpContextService httpContextService,
    IOptionsMonitor<SettingsModel> optionsMonitor)
    : IApiAuthorizationService, ISingletonService
{
    private readonly SettingsModel _settings = optionsMonitor.CurrentValue;

    /// <inheritdoc />
    public bool CheckAuthorization()
    {
        bool result;
        var username = _settings.Authentication?.ApiUsername ?? string.Empty;
        var password = _settings.Authentication?.ApiPassword ?? string.Empty;
        if (loginCookieService.CheckLoginCookie())
        {
            AddUserContext(username);
            return true;
        }

        // Try to retrieve basic auth header here.
        httpContextService.GetHeaders().TryGetCaseInsensitive(HeaderKeys.Authorization, out var value);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            value = value.Replace("Basic ", string.Empty);
            var basicAuth = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            var parts = basicAuth.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            result = username == parts[0] && password == parts[1];
        }
        catch (Exception ex)
        {
            result = false;
            logger.LogWarning(ex, InfraResources.ErrorParsingBasicAuth);
        }

        if (result)
        {
            // Everything went OK, so let's add the user to the claims.
            AddUserContext(username);
            loginCookieService.SetLoginCookie(username, password);
        }

        return result;
    }

    private void AddUserContext(string username) =>
        httpContextService.SetUser(
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) })));
}
