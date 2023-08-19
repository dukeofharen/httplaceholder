using System.Security.Claims;
using System.Text;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class ApiAuthorizationService : IApiAuthorizationService, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<ApiAuthorizationService> _logger;
    private readonly ILoginCookieService _loginCookieService;
    private readonly SettingsModel _settings;

    public ApiAuthorizationService(
        ILoginCookieService loginCookieService,
        ILogger<ApiAuthorizationService> logger,
        IHttpContextService httpContextService,
        IOptionsMonitor<SettingsModel> optionsMonitor)
    {
        _loginCookieService = loginCookieService;
        _logger = logger;
        _httpContextService = httpContextService;
        _settings = optionsMonitor.CurrentValue;
    }

    /// <inheritdoc />
    public bool CheckAuthorization()
    {
        bool result;
        var username = _settings.Authentication?.ApiUsername ?? string.Empty;
        var password = _settings.Authentication?.ApiPassword ?? string.Empty;
        if (_loginCookieService.CheckLoginCookie())
        {
            AddUserContext(username);
            return true;
        }

        // Try to retrieve basic auth header here.
        _httpContextService.GetHeaders().TryGetValue("Authorization", out var value);
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
            _logger.LogWarning(ex, "Error while parsing basic authentication.");
        }

        if (result)
        {
            // Everything went OK, so let's add the user to the claims.
            AddUserContext(username);
            _loginCookieService.SetLoginCookie(username, password);
        }

        return result;
    }

    private void AddUserContext(string username) =>
        _httpContextService.SetUser(
            new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, username)})));
}
