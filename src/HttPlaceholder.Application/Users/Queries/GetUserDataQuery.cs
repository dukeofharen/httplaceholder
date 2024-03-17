using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Authentication;
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
    public Task<UserModel> Handle(GetUserDataQuery request, CancellationToken cancellationToken)
    {
        var nameClaim = userContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        // TODO
        if (!string.IsNullOrWhiteSpace(nameClaim?.Value) && request.Username != nameClaim.Value)
        {
            throw new ForbiddenException();
        }

        return Task.FromResult(new UserModel { Username = request.Username });
    }
}
