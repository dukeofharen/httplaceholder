using System.Security.Claims;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Authorization.Implementations;

internal class UserContext : IUserContext, ITransientService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
}
