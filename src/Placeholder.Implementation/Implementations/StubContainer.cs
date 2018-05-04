using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubContainer : IStubContainer
   {
      private IEnumerable<StubModel> _stubs;
      private DateTime _stubFileModifidationDateTime;
      private readonly IConfiguration _configuration;
      private readonly ILogger<StubContainer> _logger;
      private readonly IFileService _fileService;
      private readonly IYamlService _yamlService;

      public StubContainer(
         IConfiguration configuration,
         IFileService fileService,
         ILogger<StubContainer> logger,
         IYamlService yamlService)
      {
         _configuration = configuration;
         _fileService = fileService;
         _logger = logger;
         _yamlService = yamlService;
      }

      public IEnumerable<StubModel> GetStubs()
      {
         string inputFileLocation = _configuration["inputFile"];
         if (string.IsNullOrEmpty(inputFileLocation))
         {
            throw new Exception(@"'inputFile' parameter not passed to tool. Start the application like this: placeholder --inputFile C:\tmp\stub.yml");
         }

         if (!_fileService.FileExists(inputFileLocation))
         {
            throw new Exception($"Input file '{inputFileLocation}' not found.");
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

         return _stubs;
      }
   }
}
