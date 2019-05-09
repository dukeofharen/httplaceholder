using Ducode.Essentials.Assembly.Interfaces;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// metadata controller
    /// </summary>
    [Route("ph-api/metadata")]
    public class MetadataController : BaseApiController
    {
        private readonly IAssemblyService _assemblyService;
        private readonly ILogger<MetadataController> _logger;

        /// <summary>
        /// constructor metadata controller
        /// </summary>
        /// <param name="assemblyService"></param>
        /// <param name="logger"></param>
        public MetadataController(
            IAssemblyService assemblyService,
            ILogger<MetadataController> logger)
        {
            _assemblyService = assemblyService;
            _logger = logger;
        }

        /// <summary>
        /// Get the assembly version
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
