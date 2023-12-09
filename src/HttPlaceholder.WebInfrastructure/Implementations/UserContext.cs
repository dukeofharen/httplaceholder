using System.Security.Claims;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;

namespace HttPlaceholder.WebInfrastructure.Implementations;

internal class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext, ITransientService
{
    /// <inheritdoc />
    public ClaimsPrincipal User => httpContextAccessor.HttpContext.User;
}
