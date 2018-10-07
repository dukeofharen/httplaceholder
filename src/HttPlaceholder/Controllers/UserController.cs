using System.Linq;
using System.Net;
using System.Security.Claims;
using HttPlaceholder.Filters;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace HttPlaceholder.Controllers
{
    [Route("ph-api/users")]
    [ApiAuthorization]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{username}")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, Description = "Username in claims and in URL don't match")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "OK", Type = typeof(UserModel))]
        public IActionResult Get(string username)
        {
            _logger.LogInformation($"Getting user data for '{username}'.");
            var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (!string.IsNullOrWhiteSpace(nameClaim?.Value) && username != nameClaim.Value)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var user = new UserModel
            {
                Username = username
            };
            return Ok(user);
        }
    }
}