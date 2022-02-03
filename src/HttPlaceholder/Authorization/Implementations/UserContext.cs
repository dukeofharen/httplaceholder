using System.Security.Claims;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Authorization.Implementations;

/// <inheritdoc />
internal class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
}
