using System.Threading.Tasks;
using HttPlaceholder.Application.Users.Queries;
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
    /// <returns>The user.</returns>
    [HttpGet]
    [Route("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> Get([FromRoute] string username) =>
        Ok(Map<UserDto>(await Send(new GetUserDataQuery(username))));
}
