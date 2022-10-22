using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Queries.GetConfiguration;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Configuration;
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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with the configuration.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConfigurationDto>>>
        GetConfiguration(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<ConfigurationDto>>(
            await Mediator.Send(new GetConfigurationQuery(), cancellationToken)));
}
