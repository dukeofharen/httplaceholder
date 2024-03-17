using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class LoginCookieService(
    IOptionsMonitor<SettingsModel> options,
    IHttpContextService httpContextService)
    : ILoginCookieService, ITransientService
{
    private const string LoginCookieKey = "HttPlaceholderLoggedin";
    private const string Salt = "83b2737f-7d85-4a0a-8113-b98ed4a255a1";

    /// <inheritdoc />
    public bool CheckLoginCookie()
    {
        var settings = options.CurrentValue;
        var username = settings.Authentication?.ApiUsername ?? string.Empty;
        var password = settings.Authentication?.ApiPassword ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            return true;
        }

        var expectedHash = CreateHash(username, password);
        var cookie = httpContextService.GetRequestCookie(LoginCookieKey);
        return cookie?.Value == expectedHash;
    }

    /// <inheritdoc />
    public void SetLoginCookie(string username, string password) => httpContextService.AppendCookie(
        LoginCookieKey,
        CreateHash(username, password),
        new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Lax });

    private static string CreateHash(string username, string password) =>
        HashingUtilities.GetSha512String($"{Salt}:{username}:{password}");
}
