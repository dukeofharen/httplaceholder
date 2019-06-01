using System.Security.Claims;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Implementations
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
    }
}
