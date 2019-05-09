using Ducode.Essentials.Assembly.Interfaces;
using HttPlaceholder.Models;
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
        private readonly IAssemblyService _assemblyService;

        /// <summary>
        /// constructor metadata controller
        /// </summary>
        /// <param name="assemblyService"></param>
        public MetadataController(IAssemblyService assemblyService)
        {
            _assemblyService = assemblyService;
        }

        /// <summary>
        /// Gets metadata about the API (like the assembly version).
        /// </summary>
        /// <returns>Assembly version as string</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<MetadataModel> Get()
        {
            string version = _assemblyService.GetAssemblyVersion();
            return new MetadataModel
            {
                Version = version
            };
        }
    }
}
