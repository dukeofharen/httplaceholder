using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using Microsoft.Extensions.Logging;
using HttPlaceholder.Models;

namespace HttPlaceholder.DataLogic.Implementations.StubSources
{
   internal class YamlFileStubSource : IStubSource
   {
      private IEnumerable<StubModel> _stubs;
      private DateTime _stubLoadDateTime;
      private readonly IConfigurationService _configurationService;
      private readonly ILogger<YamlFileStubSource> _logger;
      private readonly IFileService _fileService;
      private readonly IYamlService _yamlService;

      public YamlFileStubSource(
         IConfigurationService configurationService,
         IFileService fileService,
         ILogger<YamlFileStubSource> logger,
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
         config.TryGetValue(Constants.ConfigKeys.InputFileKey, out string inputFileLocation);
         var fileLocations = new List<string>();
         if (string.IsNullOrEmpty(inputFileLocation))
         {
            // If the input file location is not set, try looking in the current directory for .yml files.
            string currentDirectory = _fileService.GetCurrentDirectory();
            var yamlFiles = _fileService.GetFiles(currentDirectory, "*.yml");
            fileLocations.AddRange(yamlFiles);
         }
         else if (_fileService.IsDirectory(inputFileLocation))
         {
            var yamlFiles = _fileService.GetFiles(inputFileLocation, "*.yml");
            fileLocations.AddRange(yamlFiles);
         }
         else
         {
            fileLocations.Add(inputFileLocation);
         }

         if (fileLocations.Count == 0)
         {
            _logger.LogInformation($"No .yml input files found.");
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

               result.AddRange(_yamlService.Parse<List<StubModel>>(input));
               _stubLoadDateTime = DateTime.UtcNow;
            }

            _stubs = result;
         }
         else
         {
            _logger.LogInformation($"No stub file contents changed in the meanwhile.");
         }

         return Task.FromResult(_stubs);
      }

      private DateTime GetLastStubFileModificationDateTime(IEnumerable<string> files)
      {
         var result = files.Max(f => _fileService.GetModicationDateTime(f));
         return result;
      }
   }
}
