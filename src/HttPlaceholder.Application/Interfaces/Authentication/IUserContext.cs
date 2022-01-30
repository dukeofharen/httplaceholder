using System.Security.Claims;

namespace HttPlaceholder.Application.Interfaces.Authentication;

/// <summary>
/// Describes a class that is used to get data of the currently logged in user.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the current user.
    /// </summary>
    ClaimsPrincipal User { get; }
}
