using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
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

        public string[] GetStubRootPaths()
        {
            // First, check the "inputFile" configuration property and extract the directory of this folder.
            var inputFile = _settings.Storage?.InputFile;
            if (inputFile != null)
            {
                return inputFile.Split(new[] {"%%"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => _fileService.IsDirectory(f) ? f : Path.GetDirectoryName(f))
                    .ToArray();
            }

            // If no input file was provided, return the assembly path instead.
            return new[] {_assemblyService.GetEntryAssemblyRootPath()};
        }
    }
}
