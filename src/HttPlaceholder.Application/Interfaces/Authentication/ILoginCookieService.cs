namespace HttPlaceholder.Application.Interfaces.Authentication;

/// <summary>
///     Describes a class that is used for working with logins.
/// </summary>
public interface ILoginCookieService
{
    /// <summary>
    ///     Creates a login cookie based on the username and password.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    void SetLoginCookie(string username, string password);

    /// <summary>
    ///     Checks whether the current login cookie is valid.
    /// </summary>
    /// <returns>True if login cookie is valid, false otherwise.</returns>
    bool CheckLoginCookie();
}
