using Ducode.Essentials.Assembly.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Controllers
{
    [Route("ph-api/metadata")]
    public class MetadataController : Controller
    {
        private readonly IAssemblyService _assemblyService;
        private readonly ILogger<MetadataController> _logger;

        public MetadataController(
            IAssemblyService assemblyService,
            ILogger<MetadataController> logger)
        {
            _assemblyService = assemblyService;
            _logger = logger;
        }

        [HttpGet]
        public MetadataModel Get()
        {
            string version = _assemblyService.GetAssemblyVersion();
            return new MetadataModel
            {
                Version = version
            };
        }
    }
}
