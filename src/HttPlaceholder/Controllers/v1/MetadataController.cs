using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;
using HttPlaceholder.Application.Metadata.Queries.GetMetadata;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Web.Shared.Dto.v1.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     The metadata controller.
/// </summary>
[Route("ph-api/metadata")]
public class MetadataController : BaseApiController
{
    /// <summary>
    ///     Gets metadata about the API (like the assembly version).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>HttPlaceholder metadata.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MetadataDto>> Get(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<MetadataDto>(await Mediator.Send(new GetMetadataQuery(), cancellationToken)));

    /// <summary>
    ///     Checks whether a specific feature is enabled or not.
    /// </summary>
    /// <param name="featureFlag">The feature flag to test.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A model containing whether the feature flag is enabled or not.</returns>
    [HttpGet("features/{featureFlag}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<FeatureResultDto>> CheckFeature([FromRoute] FeatureFlagType featureFlag,
        CancellationToken cancellationToken) =>
        Ok(Mapper.Map<FeatureResultDto>(await Mediator.Send(new FeatureIsEnabledQuery(featureFlag),
            cancellationToken)));
}
