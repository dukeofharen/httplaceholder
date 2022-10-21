using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries.GetUserData;

/// <summary>
///     A query for retrieving user data of a specific user.
/// </summary>
public class GetUserDataQuery : IRequest<UserModel>
{
    /// <summary>
    ///     Constructs a <see cref="GetUserDataQuery" />
    /// </summary>
    /// <param name="username">The username.</param>
    public GetUserDataQuery(string username)
    {
        Username = username;
    }

    /// <summary>
    ///     Gets the username.
    /// </summary>
    public string Username { get; }
}
