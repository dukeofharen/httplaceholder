using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Users.Queries.GetUserData;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     The user controller.
/// </summary>
[Route("ph-api/users")]
[ApiAuthorization]
public class UserController : BaseApiController
{
    /// <summary>
    ///     Get the user for the given username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user.</returns>
    [HttpGet]
    [Route("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> Get([FromRoute] string username, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<UserDto>(await Mediator.Send(new GetUserDataQuery(username), cancellationToken)));
}
