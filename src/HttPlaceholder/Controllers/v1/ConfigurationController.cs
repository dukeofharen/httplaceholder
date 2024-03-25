using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Commands;
using HttPlaceholder.Application.Configuration.Queries;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     Controller for working with configuration in HttPlaceholder.
/// </summary>
[Route("ph-api/configuration")]
[ApiAuthorization]
public class ConfigurationController : BaseApiController
{
    /// <summary>
    ///     An endpoint that is used to retrieve the configuration of the currently running instance of HttPlaceholder.
    /// </summary>
    /// <returns>OK, with the configuration.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConfigurationDto>>>
        GetConfiguration() =>
        Ok(Map<IEnumerable<ConfigurationDto>>(
            await Send(new GetConfigurationQuery())));

    /// <summary>
    ///     An endpoint that is used to update a configuration value at runtime.
    /// </summary>
    /// <returns>No content.</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateConfigurationValue([FromBody] UpdateConfigurationValueInputDto input)
    {
        await Send(new UpdateConfigurationValueCommand(input.ConfigurationKey, input.NewValue));
        return NoContent();
    }
}
