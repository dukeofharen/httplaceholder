using System.Threading.Tasks;
using HttPlaceholder.Application.Users.Queries.GetUserData;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// User controller
/// </summary>
[Route("ph-api/users")]
[ApiAuthorization]
public class UserController : BaseApiController
{
    /// <summary>
    /// Get the user for the given username.
    /// </summary>
    /// <returns>The User.</returns>
    [HttpGet]
    [Route("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserDto>> Get([FromRoute] string username) =>
        Ok(Mapper.Map<UserDto>(await Mediator.Send(new GetUserDataQuery(username))));
}