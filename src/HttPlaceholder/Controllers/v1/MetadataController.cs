using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries;
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
    /// <returns>HttPlaceholder metadata.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MetadataDto>> Get() =>
        Ok(Map<MetadataDto>(await Send(new GetMetadataQuery())));

    /// <summary>
    ///     Checks whether a specific feature is enabled or not.
    /// </summary>
    /// <param name="featureFlag">The feature flag to test.</param>
    /// <returns>A model containing whether the feature flag is enabled or not.</returns>
    [HttpGet("features/{featureFlag}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<FeatureResultDto>> CheckFeature([FromRoute] FeatureFlagType featureFlag) =>
        Ok(Map<FeatureResultDto>(await Send(new FeatureIsEnabledQuery(featureFlag))));
}
