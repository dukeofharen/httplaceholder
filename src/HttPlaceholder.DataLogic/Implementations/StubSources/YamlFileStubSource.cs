using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.DataLogic.Implementations.StubSources
{
    internal class YamlFileStubSource : IStubSource
    {
        private IEnumerable<StubModel> _stubs;
        private DateTime _stubLoadDateTime;
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<YamlFileStubSource> _logger;
        private readonly IFileService _fileService;
        private readonly IHashingService _hashingService;
        private readonly IJsonService _jsonService;
        private readonly IYamlService _yamlService;

        public YamlFileStubSource(
           IConfigurationService configurationService,
           IFileService fileService,
           IHashingService hashingService,
           IJsonService jsonService,
           ILogger<YamlFileStubSource> logger,
           IYamlService yamlService)
        {
            _configurationService = configurationService;
            _fileService = fileService;
            _hashingService = hashingService;
            _jsonService = jsonService;
            _logger = logger;
            _yamlService = yamlService;
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            var config = _configurationService.GetConfiguration();
            config.TryGetValue(Constants.ConfigKeys.InputFileKey, out string inputFileLocation);
            var fileLocations = new List<string>();
            if (string.IsNullOrEmpty(inputFileLocation))
            {
                // If the input file location is not set, try looking in the current directory for .yml files.
                string currentDirectory = _fileService.GetCurrentDirectory();
                var yamlFiles = _fileService.GetFiles(currentDirectory, "*.yml");
                fileLocations.AddRange(yamlFiles);
            }
            else
            {
                // Split on ";": it is possible to supply multiple locations.
                var parts = inputFileLocation.Split(new[] { "%%" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string location = part.Trim();
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
                foreach (string file in fileLocations)
                {
                    // Load the stubs.
                    string input = _fileService.ReadAllText(file);
                    _logger.LogInformation($"File contents of '{file}': '{input}'");

                    var stubs = _yamlService.Parse<List<StubModel>>(input);
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

        public async Task PrepareStubSourceAsync()
        {
            // Check if the .yml files could be loaded.
            await GetStubsAsync();
        }

        private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files)
        {
            return files.Max(f => _fileService.GetModicationDateTime(f));
        }

        private void EnsureStubsHaveId(IEnumerable<StubModel> stubs)
        {
            foreach (var stub in stubs)
            {
                if (string.IsNullOrWhiteSpace(stub.Id))
                {
                    // If no ID is set, calculate a unique ID based on the stub contents.
                    string contents = _jsonService.SerializeObject(stub);
                    stub.Id = _hashingService.GetMd5String(contents);
                }
            }
        }
    }
}