using System.Security.Claims;

namespace HttPlaceholder.Application.Interfaces.Authentication
{
    public interface IUserContext
    {
        ClaimsPrincipal User { get; }
    }
}
