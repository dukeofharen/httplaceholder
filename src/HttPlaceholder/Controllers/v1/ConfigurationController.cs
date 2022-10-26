using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Commands.UpdateConfigurationValue;
using HttPlaceholder.Application.Configuration.Provider;
using HttPlaceholder.Application.Configuration.Queries.GetConfiguration;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Options;

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with the configuration.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConfigurationDto>>>
        GetConfiguration(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<ConfigurationDto>>(
            await Mediator.Send(new GetConfigurationQuery(), cancellationToken)));

    /// <summary>
    ///     An endpoint that is used to update a configuration value at runtime.
    /// </summary>
    /// <returns>No content.</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateConfigurationValue([FromBody]UpdateConfigurationValueInputDto input)
    {
        await Mediator.Send(new UpdateConfigurationValueCommand(input.ConfigurationKey, input.NewValue));
        return NoContent();
    }
}
