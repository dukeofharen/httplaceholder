using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// Controller for working with configuration in HttPlaceholder.
/// </summary>
[Route("ph-api/configuration")]
[ApiAuthorization]
public class ConfigurationController : BaseApiController
{
    /// <summary>
    /// An endpoint that is used to retrieve the configuration of the currently running instance of HttPlaceholder.
    /// </summary>
    /// <returns>OK, with the configuration.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConfigurationDto>>> GetConfiguration()
    {
        await Task.CompletedTask;
        return Ok(new[]
        {
            new ConfigurationDto
            {
                Key = "abc",
                Path = "ewqr2543:ety6354t",
                Value = "moi",
                ConfigKeyType = ConfigKeyType.Authentication
            }
        });
    }
}
