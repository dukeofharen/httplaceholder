using HttPlaceholder.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// Default base api controller
    /// </summary>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiController]
    [ApiAuthorization]
    public class BaseApiController : Controller
    {

    }
}
