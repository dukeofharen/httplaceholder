using System.Security.Claims;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Infrastructure.Web.Utilities;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Web;

internal class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext, ITransientService
{
    /// <inheritdoc />
    public ClaimsPrincipal User =>
        httpContextAccessor.GetHttpContext().User;
}
