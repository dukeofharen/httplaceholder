using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;
using HttPlaceholder.Application.Metadata.Queries.GetMetadata;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Dto.v1.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// metadata controller
/// </summary>
[Route("ph-api/metadata")]
public class MetadataController : BaseApiController
{
    /// <summary>
    /// Gets metadata about the API (like the assembly version).
    /// </summary>
    /// <returns>HttPlaceholder metadata.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MetadataDto>> Get() =>
        Ok(Mapper.Map<MetadataDto>(await Mediator.Send(new GetMetadataQuery())));

    /// <summary>
    /// Checks whether a specific feature is enabled or not.
    /// </summary>
    /// <param name="featureFlag">The feature flag to test.</param>
    /// <returns>A model containing whether the feature flag is enabled or not.</returns>
    [HttpGet("features/{featureFlag}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<FeatureResultDto>> CheckFeature([FromRoute] FeatureFlagType featureFlag) =>
        Ok(Mapper.Map<FeatureResultDto>(await Mediator.Send(new FeatureIsEnabledQuery(featureFlag))));
}