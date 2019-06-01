using System.Security.Claims;

namespace HttPlaceholder.Application.Interfaces
{
    public interface IUserContext
    {
        ClaimsPrincipal User { get; }
    }
}
