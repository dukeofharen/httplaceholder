using System.Threading.Tasks;
using HttPlaceholder.Application.Users.Queries.GetUserData;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// User controller
    /// </summary>
    [Route("ph-api/users")]
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
        public async Task<ActionResult<UserModel>> Get([FromRoute]GetUserDataQuery query) =>
            await Mediator.Send(query);
    }
}
