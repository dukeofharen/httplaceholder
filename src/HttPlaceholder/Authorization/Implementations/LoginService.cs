using System.Linq;
using Ducode.Essentials.Console;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Authorization.Implementations
{
    internal class LoginService : ILoginService
    {
        private const string Salt = "83b2737f-7d85-4a0a-8113-b98ed4a255a1";
        private readonly IConfigurationService _configurationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(
            IConfigurationService configurationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _configurationService = configurationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool CheckLoginCookie()
        {
            var config = _configurationService.GetConfiguration();
            string username = config.GetValue(Constants.ConfigKeys.ApiUsernameKey, string.Empty);
            string password = config.GetValue(Constants.ConfigKeys.ApiPasswordKey, string.Empty);
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                return true;
            }

            string expectedHash = CreateHash(username, password);
            var cookie = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == Constants.CookieKeys.LoginCookieKey);
            return cookie.Value == expectedHash;
        }

        public void SetLoginCookie(string username, string password)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                Constants.CookieKeys.LoginCookieKey,
                CreateHash(username, password),
                new CookieOptions { HttpOnly = true });
        }

        private string CreateHash(string username, string password) =>
            HashingUtilities.GetSha512String($"{Salt}:{username}:{password}");
    }
}
