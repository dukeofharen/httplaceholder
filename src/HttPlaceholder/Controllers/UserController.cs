using System.Linq;
using System.Security.Claims;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// User controller
    /// </summary>
    [Route("ph-api/users")]
    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor for the <see cref="UserController"/>
        /// </summary>
        /// <param name="logger"></param>
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get the user for the given username.
        /// </summary>
        /// <param name="username">The user to search for.</param>
        /// <returns>The User.</returns>
        [HttpGet]
        [Route("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<UserModel> Get(string username)
        {
            _logger.LogInformation($"Getting user data for '{username}'.");
            var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (!string.IsNullOrWhiteSpace(nameClaim?.Value) && username != nameClaim.Value)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var user = new UserModel
            {
                Username = username
            };
            return Ok(user);
        }
    }
}