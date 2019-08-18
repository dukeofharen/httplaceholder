using System.Threading.Tasks;
using HttPlaceholder.Application.Metadata.Queries.GetMetadata;
using HttPlaceholder.Dto.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers
{
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
    }
}
