using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Services;
using Microsoft.Extensions.Logging;
using Placeholder.Models;

namespace Placeholder.DataLogic.Implementations.StubSources
{
   internal class YamlFileStubSource : IStubSource
   {
      private IEnumerable<StubModel> _stubs;
      private DateTime _stubFileModifidationDateTime;
      private readonly IConfigurationService _configurationService;
      private readonly ILogger<StubContainer> _logger;
      private readonly IFileService _fileService;
      private readonly IYamlService _yamlService;

      public YamlFileStubSource(
         IConfigurationService configurationService,
         IFileService fileService,
         ILogger<StubContainer> logger,
         IYamlService yamlService)
      {
         _configurationService = configurationService;
         _fileService = fileService;
         _logger = logger;
         _yamlService = yamlService;
      }

      public Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         var config = _configurationService.GetConfiguration();
         string inputFileLocation = config["inputFile"];
         string currentDirectory = _fileService.GetCurrentDirectory();
         if (string.IsNullOrEmpty(inputFileLocation))
         {
            // If the input file location is not set, try looking in the current directory for .yml files.
            var yamlFiles = _fileService.GetFiles(currentDirectory, "*.yml");
            if (yamlFiles.Length > 1)
            {
               throw new Exception($"Multiple .yml files found in '{currentDirectory}'; no idea which to pick.");
            }

            inputFileLocation = yamlFiles.Single();
         }

         if (!_fileService.FileExists(inputFileLocation))
         {
            inputFileLocation = Path.Combine(currentDirectory, inputFileLocation);
            if (!_fileService.FileExists(inputFileLocation))
            {
               throw new Exception($"Input file '{inputFileLocation}' not found.");
            }
         }

         var stubFileModifidationDateTime = _fileService.GetModicationDateTime(inputFileLocation);
         _logger.LogInformation($"Last modification date time of '{inputFileLocation}': '{stubFileModifidationDateTime}'");
         if (_stubs == null || stubFileModifidationDateTime > _stubFileModifidationDateTime)
         {
            // Load the stubs.
            string input = _fileService.ReadAllText(inputFileLocation);
            _logger.LogInformation($"File contents of '{inputFileLocation}': '{input}'");

            _stubs = _yamlService.Parse<List<StubModel>>(input);
            _stubFileModifidationDateTime = stubFileModifidationDateTime;
         }
         else
         {
            _logger.LogInformation($"File contents of '{inputFileLocation}' not changed in the meanwhile.");
         }

         return Task.FromResult(_stubs);
      }
   }
}
