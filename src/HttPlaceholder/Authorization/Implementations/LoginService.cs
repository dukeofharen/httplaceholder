using System.Linq;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Authorization.Implementations
{
    internal class LoginService : ILoginService
    {
        private const string Salt = "83b2737f-7d85-4a0a-8113-b98ed4a255a1";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SettingsModel _settings;

        public LoginService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<SettingsModel> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _settings = options.Value;
        }

        public bool CheckLoginCookie()
        {
            var username = _settings.Authentication?.ApiUsername ?? string.Empty;
            var password = _settings.Authentication?.ApiPassword ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                return true;
            }

            var expectedHash = CreateHash(username, password);
            var cookie = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == CookieKeys.LoginCookieKey);
            return cookie.Value == expectedHash;
        }

        public void SetLoginCookie(string username, string password)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                CookieKeys.LoginCookieKey,
                CreateHash(username, password),
                new CookieOptions { HttpOnly = true });
        }

        private string CreateHash(string username, string password) =>
            HashingUtilities.GetSha512String($"{Salt}:{username}:{password}");
    }
}
