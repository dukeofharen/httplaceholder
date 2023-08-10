namespace HttPlaceholder.Application.Interfaces.Authentication;

/// <summary>
///     Describes a class that is used to check the authentication and authorization for all API requests.
/// </summary>
public interface IApiAuthorizationService
{
    /// <summary>
    ///     Checks the authorization of the current request.
    /// </summary>
    /// <returns>True if authorization succeeded; false otherwise.</returns>
    bool CheckAuthorization();
}
