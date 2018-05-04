using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubContainer : IStubContainer
   {
      private IEnumerable<StubModel> _stubs;
      private DateTime _stubFileModifidationDateTime;
      private readonly IConfiguration _configuration;
      private readonly IFileService _fileService;
      private readonly IYamlService _yamlService;

      public StubContainer(
         IConfiguration configuration,
         IFileService fileService,
         IYamlService yamlService)
      {
         _configuration = configuration;
         _fileService = fileService;
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
         if (_stubs == null || stubFileModifidationDateTime > _stubFileModifidationDateTime)
         {
            // Load the stubs.
            string input = _fileService.ReadAllText(inputFileLocation);
            _stubs = _yamlService.Parse<List<StubModel>>(input);
            _stubFileModifidationDateTime = stubFileModifidationDateTime;
         }

         return _stubs;
      }
   }
}
