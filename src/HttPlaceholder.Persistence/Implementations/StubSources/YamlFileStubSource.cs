using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class YamlFileStubSource : IStubSource
    {
        private IEnumerable<StubModel> _stubs;
        private DateTime _stubLoadDateTime;
        private readonly ILogger<YamlFileStubSource> _logger;
        private readonly IFileService _fileService;
        private readonly SettingsModel _settings;

        public YamlFileStubSource(
           IFileService fileService,
           ILogger<YamlFileStubSource> logger,
           IOptions<SettingsModel> options)
        {
            _fileService = fileService;
            _logger = logger;
            _settings = options.Value;
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            var inputFileLocation = _settings.Storage?.InputFile;
            var fileLocations = new List<string>();
            if (string.IsNullOrEmpty(inputFileLocation))
            {
                // If the input file location is not set, try looking in the current directory for .yml files.
                var currentDirectory = _fileService.GetCurrentDirectory();
                var yamlFiles = _fileService.GetFiles(currentDirectory, "*.yml");
                fileLocations.AddRange(yamlFiles);
            }
            else
            {
                // Split on ";": it is possible to supply multiple locations.
                var parts = inputFileLocation.Split(new[] { "%%" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var location = part.Trim();
                    _logger.LogInformation($"Reading location '{location}'.");
                    if (_fileService.IsDirectory(location))
                    {
                        var yamlFiles = _fileService.GetFiles(location, "*.yml");
                        fileLocations.AddRange(yamlFiles);
                    }
                    else
                    {
                        fileLocations.Add(location);
                    }
                }
            }

            if (fileLocations.Count == 0)
            {
                _logger.LogInformation("No .yml input files found.");
                return Task.FromResult(new StubModel[0].AsEnumerable());
            }

            if (_stubs == null || GetLastStubFileModificationDateTime(fileLocations) > _stubLoadDateTime)
            {
                var result = new List<StubModel>();
                foreach (var file in fileLocations)
                {
                    // Load the stubs.
                    var input = _fileService.ReadAllText(file);
                    _logger.LogInformation($"File contents of '{file}': '{input}'");

                    var stubs = YamlUtilities.Parse<List<StubModel>>(input);
                    EnsureStubsHaveId(stubs);
                    result.AddRange(stubs);
                    _stubLoadDateTime = DateTime.UtcNow;
                }

                _stubs = result;
            }
            else
            {
                _logger.LogInformation("No stub file contents changed in the meanwhile.");
            }

            return Task.FromResult(_stubs);
        }

        public async Task PrepareStubSourceAsync() =>
            // Check if the .yml files could be loaded.
            await GetStubsAsync();

        private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files) => files.Max(f => _fileService.GetModicationDateTime(f));

        private static void EnsureStubsHaveId(IEnumerable<StubModel> stubs)
        {
            foreach (var stub in stubs)
            {
                if (!string.IsNullOrWhiteSpace(stub.Id))
                {
                    continue;
                }

                // If no ID is set, calculate a unique ID based on the stub contents.
                var contents = JsonConvert.SerializeObject(stub);
                stub.Id = HashingUtilities.GetMd5String(contents);
            }
        }
    }
}
