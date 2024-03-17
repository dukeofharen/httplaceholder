using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Users.Queries;

/// <summary>
///     A query for retrieving user data of a specific user.
/// </summary>
public class GetUserDataQuery(string username) : IRequest<UserModel>
{
    /// <summary>
    ///     Gets the username.
    /// </summary>
    public string Username { get; } = username;
}

/// <summary>
///     A query handler for retrieving user data of a specific user.
/// </summary>
public class GetUserDataQueryHandler(IUserContext userContext) : IRequestHandler<GetUserDataQuery, UserModel>
{
    /// <inheritdoc />
    public Task<UserModel> Handle(GetUserDataQuery request, CancellationToken cancellationToken) =>
        userContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
            .If(c => !string.IsNullOrWhiteSpace(c?.Value) && request.Username != c.Value, _ => throw new ForbiddenException())
            .Map(_ => Task.FromResult(new UserModel { Username = request.Username }));
}
