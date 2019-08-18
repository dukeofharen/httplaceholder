using System.IO;
using Ducode.Essentials.Assembly.Interfaces;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations
{
    internal class StubRootPathResolver : IStubRootPathResolver
    {
        private readonly IAssemblyService _assemblyService;
        private readonly SettingsModel _settings;
        private readonly IFileService _fileService;

        public StubRootPathResolver(
           IAssemblyService assemblyService,
           IFileService fileService,
           IOptions<SettingsModel> options)
        {
            _assemblyService = assemblyService;
            _fileService = fileService;
            _settings = options.Value;
        }

        public string GetStubRootPath()
        {
            // First, check the "inputFile" configuration property and extract the directory of this folder.
            string inputFile = _settings.Storage?.InputFile;
            if (inputFile != null)
            {
                return
                   _fileService.IsDirectory(inputFile) ?
                   inputFile :
                   Path.GetDirectoryName(inputFile);
            }

            // If no input file was provided, return the assembly path instead.
            return _assemblyService.GetEntryAssemblyRootPath();
        }
    }
}
